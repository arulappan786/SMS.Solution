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

namespace SMS.Application.CQRS.Core.Students.Commands.CreateStudent
{
    public class CreateStudentCommandHandler(IStudentRepository studentRepository,
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
        public async Task<ServiceResponse> Handle(CreateStudentCommand createStudentRequest, CancellationToken cancellationToken)
        {
            // Validating user input.
            logger.LogInfo("Starting student onboarding: Validating input.");
            var validationResult = await validationService.ValidateAsync(createStudentRequest, validator);
            if (!validationResult.Success) return validationResult;

            // Checking uniqueness of the student both in student and user store through email.
            logger.LogInfo($"Checking global uniqueness for email: {createStudentRequest.Email}");
            if (!await onboardingService.IsUniqueAsync(createStudentRequest.Email, cancellationToken))
            {
                return new ServiceResponse()
                {
                    Success = false,
                    Message = $"Student with the email {createStudentRequest.Email} already exists in the system or user store."
                };
            }

            AppUser? newUser = null;
            string newPassword = passwordGeneratorService.GenerateSecurePassword();

            try
            {
                // Creating a new user and assigning Student role.
                logger.LogInfo("Creating user account and assigning role atomically.");
                newUser = await onboardingService.CreateUserAndAssignRoleAsync(createStudentRequest, newPassword, cancellationToken);

                // Preparing student data prior to adding the student to the student store.
                if (!Guid.TryParse(newUser.Id, out Guid userIdGuid))
                    throw new Exception($"System error: UserId {newUser.Id} could not be converted to Guid for student record.");

                var newStudentCode = await studentCodeGeneratorService.GenerateNewStudentCodeAsync(DateTime.UtcNow);
                ((IStudentHasInternalIds)createStudentRequest).UserId = userIdGuid;
                ((IStudentHasInternalIds)createStudentRequest).StudentCode = newStudentCode;
                var mappedStudent = mapper.Map<Student>(createStudentRequest);

                // Adding the student to the student store.
                await studentRepository.AddAsync(mappedStudent);

                // Commiting the identity and student transactions.
                var result = await unitOfWork.CommitAsync(cancellationToken);

                if (result <= 0)
                    throw new InvalidOperationException($"Student record creation failed for {createStudentRequest.Email} during final commit.");

                try
                {
                    logger.LogInfo("Sending welcome email with credentials.");
                    await emailService.SendGmailAsync(createStudentRequest.Email, newUser.UserName!, newPassword, true);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to send welcome email to {createStudentRequest.Email}. Manual intervention required.");
                }

                return new ServiceResponse()
                {
                    Success = true,
                    Message = $"New student is created with the email: {createStudentRequest.Email}. Credentials email sent."
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