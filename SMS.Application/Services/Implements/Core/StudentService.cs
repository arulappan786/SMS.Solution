using AutoMapper;
using FluentValidation;
using SMS.Application.CQRS.Core.Students.Commands;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Interfaces.Common;
using SMS.Application.Services.Interfaces.Core;
using SMS.Application.Services.Interfaces.Logging;
using SMS.Application.Validations;
using SMS.Domain.Entities.Core;
using SMS.Domain.Entities.Identity;
using SMS.Domain.Interfaces.Repositories;

namespace SMS.Application.Services.Implements.Core
{
    public class StudentService(
        IStudentRepository studentRepository,
        IUnitOfWork unitOfWork,
        IPasswordGeneratorService passwordGeneratorService,
        IStudentCodeGeneratorService studentCodeGeneratorService,
        IValidationService validationService,
        IValidator<CreateStudentCommand> validator,
        IAppLogger<StudentService> logger,
        IMapper mapper,
        IStudentOnboardingService onboardingService, // NEW Service for SRP
        IEmailService emailService) : IStudentService // NEW Service for Email
    {
      
        /// <summary>
        /// To onboard a new student into the system. This method acts as the Orchestrator.
        /// </summary>
        /// <param name="student"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ServiceResponse> OnboardNewStudentAsync(CreateStudentCommand student, CancellationToken cancellationToken)
        {
            // The Orchestrator's primary job is flow control and transaction coordination.

            // 1. VALIDATE INPUT (SRP Adherence)
            logger.LogInfo("Starting student onboarding: Validating input.");
            var validationResult = await validationService.ValidateAsync(student, validator);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            // 2. UNIQUNESS CHECK (SRP Adherence using dedicated Onboarding Service)
            logger.LogInfo($"Checking global uniqueness for email: {student.Email}");
            if (!await onboardingService.IsUniqueAsync(student.Email, cancellationToken))
            {
                return new ServiceResponse()
                {
                    Success = false,
                    Message = $"Student with the email {student.Email} already exists in the system or user store."
                };
            }

            AppUser? newUser = null;
            string newPassword = passwordGeneratorService.GenerateSecurePassword();

            try
            {
                // 3. ATOMIC USER & ROLE CREATION (Identity Persistence - Transactional)
                // This step calls the OnboardingService, which manages the AppUser/Role transaction.
                // The OnboardingService ensures that user and role creation are atomic.
                logger.LogInfo("Creating user account and assigning role atomically.");

                // This call relies on the OnboardingService (File 2) which handles its own Begin/Rollback.
                newUser = await onboardingService.CreateUserAndAssignRoleAsync(
                    student,
                    newPassword,
                    cancellationToken);

                // --- 4. CREATE STUDENT RECORD (Core Persistence - Part of main UnitOfWork) ---

                // a. Prepare Student Data
                if (!Guid.TryParse(newUser.Id, out Guid userIdGuid))
                {
                    // If the user was created but ID parsing failed (a serious issue), throw to trigger rollback
                    throw new Exception($"System error: UserId {newUser.Id} could not be converted to Guid for student record.");
                }

                ((IStudentHasInternalIds)student).UserId = userIdGuid;
                ((IStudentHasInternalIds)student).StudentCode = await studentCodeGeneratorService.GenerateNewStudentCodeAsync(DateTime.UtcNow);

                var mappedStudent = mapper.Map<Student>(student);

                // b. Add to Student Repository (tracked by main UnitOfWork)
                await studentRepository.AddAsync(mappedStudent);

                // c. COMMIT CORE UNIT OF WORK (Saves Student Record and commits any pending Identity changes)
                // Assuming IUnitOfWork is the wrapper for AppDbContext/EFCore, this commits the Student record change.
                var result = await unitOfWork.CommitAsync(cancellationToken);

                if (result <= 0)
                {
                    // Though user/role are committed by the OnboardingService, 
                    // a failure here means the business logic is incomplete.
                    // If Commit fails, it typically throws an exception, but checking return value is safer.
                    throw new InvalidOperationException($"Student record creation failed for {student.Email} during final commit.");
                }

                // --- 5. POST-COMMIT ACTION (SRP Adherence for External Comms) ---
                // This operation is intentionally OUTSIDE the main transaction (ACID boundary).
                // Failure here does NOT roll back the successful user/student record creation.

                //try
                //{
                //    logger.LogInfo("Sending welcome email with credentials.");
                //    await emailService.SendWelcomeEmailAsync(
                //        student.Email,
                //        newUser.UserName!,
                //        newPassword,
                //        StudentRole);
                //}
                //catch (Exception ex)
                //{
                //    // Log failure, but the business process is fundamentally complete.
                //    logger.LogError(ex, $"Failed to send welcome email to {student.Email}. Manual intervention required.");
                //}


                return new ServiceResponse()
                {
                    Success = true,
                    Message = $"New student is created with the email: {student.Email}. Credentials email sent."
                };
            }
            catch (Exception ex)
            {
                // The outer exception handling catches any fatal error from either the Identity transaction 
                // or the Core transaction (step 4).

                // IMPORTANT: The OnboardingService handles its own Rollback.
                // If the error occurred in step 4 or post-commit, there's nothing to rollback here
                // unless IUnitOfWork implements transaction scope explicitly. We rely on the inner service
                // and the nature of EF Core to manage the persistence failures.

                logger.LogError(ex, "Fatal error during student onboarding flow.");

                // Note: The logic for deleting the half-created user if the StudentRecord failed 
                // is complex and often deferred to a compensating transaction or manual cleanup.

                return new ServiceResponse()
                {
                    Success = false,
                    Message = $"Error while onboarding a new student into the system. {ex.Message}"
                };
            }
        }
    }
}