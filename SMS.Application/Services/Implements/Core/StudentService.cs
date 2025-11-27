using AutoMapper;
using FluentValidation;
using SMS.Application.CQRS.Core.Students.Commands;
using SMS.Application.CQRS.Core.Students.Queries;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Core;
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
        IStudentOnboardingService onboardingService,
        IEmailSenderService emailService) : IStudentService
    {
        /// <summary>
        /// To fetch all the students from the student store.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PaginatedResultDto<StudentDto>> GetAllStudentAsync(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {
            //var students = await studentRepository.GetAllAsync(cancellationToken);
            //var mappedStudents = mapper.Map<IEnumerable<StudentDto>>(students);
            //return mappedStudents;

            // 1. Call Repository to get paged data and total count
            var (students, totalCount) = await studentRepository.GetAllPaginatedAsync(
                request.PageNumber, request.PageSize, cancellationToken);

            // 2. Map the entities to DTOs
            var studentDtos = mapper.Map<IEnumerable<StudentDto>>(students);

            // 3. Construct the final PaginatedResultDto
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PaginatedResultDto<StudentDto>
            {
                Items = studentDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }       

        /// <summary>
        /// To fetch a specific student by studentid.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<StudentDto> GetStudentByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var student = await studentRepository.GetAsync(id, cancellationToken);
            var mappedStudent = mapper.Map<StudentDto>(student);
            return mappedStudent;
        }

        /// <summary>
        /// To onboard a new student into the system. This method acts as the Orchestrator.
        /// </summary>
        /// <param name="student"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ServiceResponse> OnboardNewStudentAsync(CreateStudentCommand student, CancellationToken cancellationToken)
        {
            // Validating user input.
            logger.LogInfo("Starting student onboarding: Validating input.");
            var validationResult = await validationService.ValidateAsync(student, validator);
            if (!validationResult.Success) return validationResult;

            // Checking uniqueness of the student both in student and user store through email.
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
                // Creating a new user and assigning Student role.
                logger.LogInfo("Creating user account and assigning role atomically.");
                newUser = await onboardingService.CreateUserAndAssignRoleAsync(student, newPassword, cancellationToken);

                // Preparing student data prior to adding the student to the student store.
                if (!Guid.TryParse(newUser.Id, out Guid userIdGuid))
                    throw new Exception($"System error: UserId {newUser.Id} could not be converted to Guid for student record.");
                
                var newStudentCode = await studentCodeGeneratorService.GenerateNewStudentCodeAsync(DateTime.UtcNow);
                ((IStudentHasInternalIds)student).UserId = userIdGuid;
                ((IStudentHasInternalIds)student).StudentCode = newStudentCode;
                var mappedStudent = mapper.Map<Student>(student);

                // Adding the student to the student store.
                await studentRepository.AddAsync(mappedStudent);

                // Commiting the identity and student transactions.
                var result = await unitOfWork.CommitAsync(cancellationToken);

                if (result <= 0)
                    throw new InvalidOperationException($"Student record creation failed for {student.Email} during final commit.");

                try
                {
                    logger.LogInfo("Sending welcome email with credentials.");
                    await emailService.SendGmailAsync(student.Email, newUser.UserName!, newPassword, true);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to send welcome email to {student.Email}. Manual intervention required.");
                }

                return new ServiceResponse()
                {
                    Success = true,
                    Message = $"New student is created with the email: {student.Email}. Credentials email sent."
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