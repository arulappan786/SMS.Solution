using AutoMapper;
using FluentValidation;
using Hangfire;
using MediatR;
using SMS.Application.DTOs.Core.Students;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Common;
using SMS.Application.Services.Core.Students;
using SMS.Application.Services.Jobs;
using SMS.Application.Services.Logging;
using SMS.Domain.Constants;
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
                                             IValidator<CreateStudentCommand> validator,
                                             IAppLogger<CreateStudentCommandHandler> logger,
                                             IMapper mapper,
                                             IStudentOnboardingService onboardingService,
                                             IEmailTemplatesLoader templateLoader)
        : IRequestHandler<CreateStudentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            AppUser? newUser = null;
            string newPassword = passwordGeneratorService.GenerateSecurePassword();

            try
            {
                // --- 1. Validation ---
                logger.LogInfo("Starting student onboarding: Validating input.");
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    logger.LogWarning($"Student onboarding failed validation for {request.Email}.");
                    return ServiceResponse.Failure("Validation failed for student command.", validationResult.Errors);
                }

                // --- 2. Uniqueness Check ---
                logger.LogInfo($"Checking global uniqueness for email: {request.Email}");
                if (!await onboardingService.IsUniqueAsync(request.Email, cancellationToken))
                {
                    return ServiceResponse.Failure($"Student with the email {request.Email} already exists in the system or user store.");
                }

                // --- 3. Identity Creation ---
                logger.LogInfo("Creating user account and assigning role atomically.");
                // This call is crucial: if it fails, the process must stop and no core entity created.
                newUser = await onboardingService.CreateUserAndAssignRoleAsync(request, newPassword, cancellationToken);

                // --- 4. Prepare Student Core Data ---
                logger.LogInfo("Preparing student core entity data.");

                if (!Guid.TryParse(newUser.Id, out Guid userIdGuid))
                {
                    // This indicates a severe issue in the Identity system's output
                    throw new InvalidOperationException($"Identity system failed to return a valid GUID for UserId: {newUser.Id}");
                }

                var newStudentCode = await studentCodeGeneratorService.GenerateNewStudentCodeAsync(DateTime.UtcNow);

                // Set the internal IDs on the command before mapping
                ((IStudentHasInternalIds)request).UserId = userIdGuid;
                ((IStudentHasInternalIds)request).StudentCode = newStudentCode;

                var mappedStudent = mapper.Map<Student>(request);

                // --- 5. Student Creation (Tracked) ---
                await repository.AddAsync(mappedStudent, cancellationToken);

                // --- 6. Commit Transactions (Student & Identity Stores) ---
                logger.LogInfo("Attempting to commit core student and identity entity transactions.");
                var result = await unitOfWork.CommitAsync(cancellationToken);

                if (result <= 0)
                {
                    // This often means a concurrency issue or zero records affected. Must be cleaned up.
                    throw new InvalidOperationException($"Student record creation failed for {request.Email} during final commit (Zero records affected).");
                }

                // --- 7. Linking Student Profile (Post-Commit) ---
                // Linking is done using the ID from the successfully committed core entity.
                try
                {
                    Guid studentId = mappedStudent.Id;
                    logger.LogInfo($"Associating Student ID {studentId} with User ID {newUser.Id}.");

                    bool updateResult = await onboardingService.LinkStudentProfileToUserAsync(newUser, studentId, cancellationToken);

                    if (!updateResult)
                    {
                        // Log a severe warning: Core record is OK, but linking failed.
                        logger.LogError($"FAILED to link Student ID {studentId} to User ID {newUser.Id}. Manual intervention required.");
                    }
                }
                catch (Exception ex)
                {
                    // Log the linking failure but do NOT re-throw, as the core process succeeded.
                    logger.LogError(ex, "Associating student profile with user account failed. Manual intervention required.");
                }

                // --- 8. Decoupled Actions (Email) ---
                try
                {
                    logger.LogInfo("Enqueuing welcome email job via Hangfire.");
                    string htmlBody = templateLoader.LoadEmailTemplate(request, newPassword, EmailTemplates.WelcomeUserTemplate);

                    // The key line: Hangfire finds the EmailJobService via DI later.
                    BackgroundJob.Enqueue<IEmailJobService>(job =>
                        job.SendWelcomeEmailAsync(request.Email, $"Welcome {request.FullName}", htmlBody, true));

                    logger.LogInfo($"Welcome email job successfully enqueued for {request.Email}.");
                }
                catch (Exception ex)
                {
                    // Log fatal error if Hangfire can't even enqueue the job (DB connection, serialization issue)
                    logger.LogCritical(ex, $"FATAL: Could not enqueue email job for {request.Email}. Manual intervention required.");
                }

                // --- 9. Success ---
                return ServiceResponse.Success($"New student is created with the email: {request.Email}. Credentials email process initiated.",
                                               new CreatedStudentDto(StudentId: mappedStudent.Id, UserId: newUser.Id, StudentCode: newStudentCode));
            }
            catch (Exception ex)
            {
                // --- 10. Centralized Error Handling and Rollback ---
                logger.LogError(ex, $"Critical error while onboarding student {request.Email}. Initiating cleanup.");

                // CRITICAL SAFETY CHECK: If newUser was successfully created but the commit failed, delete the orphan user account.
                if (newUser != null)
                {
                    try
                    {
                        await onboardingService.RollbackUserCreationAsync(newUser);
                        logger.LogWarning($"Rollback completed for AppUser: {newUser.Id}. Orphan account deleted.");
                    }
                    catch (Exception rollbackEx)
                    {
                        logger.LogCritical(rollbackEx, $"FATAL: Failed to rollback user creation for {newUser.Id}. Manual cleanup is mandatory.");
                    }
                }

                await unitOfWork.RollbackAsync(cancellationToken); // Ensure any pending EF changes are discarded.

                // Use ServiceResponse.Failure
                return ServiceResponse.Failure($"Error while onboarding a new student into the system. {ex.Message}");
            }
        }
    }
}