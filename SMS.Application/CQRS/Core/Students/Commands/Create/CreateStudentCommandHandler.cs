using AutoMapper;
using FluentValidation;
using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Common;
using SMS.Application.Services.Core.Students;
using SMS.Application.Services.Logging;
using SMS.Application.Validations;
using SMS.Domain.Entities.Core;
using SMS.Domain.Entities.Identity;
using SMS.Domain.Interfaces.Repositories.Common;
using SMS.Domain.Interfaces.Repositories.Core;

namespace SMS.Application.CQRS.Core.Students.Commands.Create
{
    public class CreateStudentCommandHandler(IStudentRepository repository,
        IUnitOfWork unitOfWork,
        IPasswordGeneratorService passwordGeneratorService,
        IStudentCodeGeneratorService studentCodeGeneratorService,
        IValidationService validationService,
        IValidator<CreateStudentCommand> validator,
        IAppLogger<CreateStudentCommandHandler> logger,
        IMapper mapper,
        IStudentOnboardingService onboardingService,
        IEmailSenderService emailService) : IRequestHandler<CreateStudentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            // Validating user input.
            logger.LogInfo("Starting student onboarding: Validating input.");
            var validationResult = await validationService.ValidateAsync(request, validator);
            if (!validationResult.Success) return validationResult;

            // Checking uniqueness of the student both in student and user store through email.
            logger.LogInfo($"Checking global uniqueness for email: {request.Email}");
            if (!await onboardingService.IsUniqueAsync(request.Email, cancellationToken))
            {
                return new ServiceResponse()
                {
                    Success = false,
                    Message = $"Student with the email {request.Email} already exists in the system or user store."
                };
            }

            AppUser? newUser = null;
            string newPassword = passwordGeneratorService.GenerateSecurePassword();

            try
            {
                // Creating a new user and assigning Student role.
                logger.LogInfo("Creating user account and assigning role atomically.");
                newUser = await onboardingService.CreateUserAndAssignRoleAsync(request, newPassword, cancellationToken);

                // Preparing student data prior to adding the student to the student store.
                if (!Guid.TryParse(newUser.Id, out Guid userIdGuid))
                    throw new Exception($"System error: UserId {newUser.Id} could not be converted to Guid for student record.");

                var newStudentCode = await studentCodeGeneratorService.GenerateNewStudentCodeAsync(DateTime.UtcNow);
                ((IStudentHasInternalIds)request).UserId = userIdGuid;
                ((IStudentHasInternalIds)request).StudentCode = newStudentCode;
                var mappedStudent = mapper.Map<Student>(request);

                // Adding the student to the student store.
                await repository.AddAsync(mappedStudent, cancellationToken);

                // Commiting the identity and student transactions.
                var result = await unitOfWork.CommitAsync(cancellationToken);

                if (result <= 0)
                    throw new InvalidOperationException($"Student record creation failed for {request.Email} during final commit.");

                // Linking the newly created student profile with the newly created user account.
                try
                {
                    var newStudent = await repository.GetByEmailAsync(request.Email, cancellationToken);
                    if (newStudent != null)
                    {
                        logger.LogInfo("Associating newly created student with the newly created user account.");
                        var updateResult = await onboardingService.LinkStudentProfileToUserAsync(newUser, newStudent.Id, cancellationToken);
                        if(!updateResult)
                        {
                            throw new Exception($"Associating newly created student with the newly created user account failed for email: {request.Email}.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Associating newly creating student with the newly created user account failed.");
                }

                // Sending welcome email with credentials.
                try
                {
                    logger.LogInfo("Sending welcome email with credentials.");
                    var emailResult = await emailService.SendGmailAsync(request.Email, newUser.UserName!, newPassword, true);
                    if (!emailResult)
                    {
                        throw new Exception($"Sending welcome email failed for {request.Email}.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to send welcome email to {request.Email}. Manual intervention required.");
                }

                // Returning successful response.
                return new ServiceResponse()
                {
                    Success = true,
                    Message = $"New student is created with the email: {request.Email}. Credentials email sent."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while onboarding a new student into the system.");

                return new ServiceResponse()
                {
                    Success = false,
                    Message = $"Error while onboarding a new student into the system. {ex.Message}"
                };
            }
        }
    }
}