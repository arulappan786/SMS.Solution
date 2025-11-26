using AutoMapper;
using FluentValidation;
using SMS.Application.CQRS.Core.Students.Commands;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Interfaces.Common;
using SMS.Application.Services.Interfaces.Core;
using SMS.Application.Services.Interfaces.Identity;
using SMS.Application.Services.Interfaces.Logging;
using SMS.Application.Validations;
using SMS.Domain.Entities.Core;
using SMS.Domain.Entities.Identity;
using SMS.Domain.Interfaces.Repositories;

namespace SMS.Application.Services.Implements.Core
{
    public class StudentService(IStudentRepository studentRepository,
                                IUnitOfWork unitOfWork,
                                IUserManagementService userManagement,
                                IRoleManagementService roleManagement,
                                IPasswordGeneratorService passwordGeneratorService,
                                IStudentCodeGeneratorService studentCodeGeneratorService,
                                IValidationService validationService,
                                IValidator<CreateStudentCommand> validator,
                                IAppLogger<StudentService> logger,
                                IMapper mapper) : IStudentService
    {
        /// <summary>
        /// To onboard a new student into the system.
        /// </summary>
        /// <param name="student"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ServiceResponse> CreateStudentAsync(CreateStudentCommand student, CancellationToken cancellationToken)
        {
            try
            {

                // (1/7) => Validating user input towards new student onboarding.
                logger.LogInfo($"(1/7) => Validating user input towards new student onboarding.");
                var validationResult = await validationService.ValidateAsync(student, validator);
                if (!validationResult.Success)
                    return validationResult;

                // (2/7) => Checking if the same student is already present in the student store.
                logger.LogInfo($"(2/7) => Checking if the same student is already present in the student store.");
                var studentByEmail = await studentRepository.GetByEmailAsync(student.Email);
                if (studentByEmail != null)
                    return new ServiceResponse()
                    {
                        Success = false,
                        Message = $"Student with the same email {student.Email} exists already."
                    };

                // (3/7) => Checking if the same student with the same email exists already in the user store.
                logger.LogInfo($"(3/7) => Checking if the same student with the same email exists already in the user store.");
                var userByStudentMail = await userManagement.GetUserByEmailAsync(student.Email);
                if (userByStudentMail != null)
                    return new ServiceResponse()
                    {
                        Success = false,
                        Message = $"Student with the same email {student.Email} exists already. " +
                        $"Please check for the user existence with this email {student.Email}"
                    };

                // (4/7) => Creating new user account for the student with a temproray password with the given student email address.
                logger.LogInfo($"(4/7) => Creating new user account for the student with a temproray password with the given student email address.");
                string newPassword = GenerateNewPassword();
                var userCreationResult = await userManagement.CreateUserAsync(new AppUser()
                {
                    Email = student.Email,
                    PasswordHash = newPassword,
                    DisplayName = student.FullName.ToString(),
                    UserName = student.Email
                });

                if (!userCreationResult)
                    return new ServiceResponse()
                    {
                        Success = false,
                        Message = $"Error while creating new user entry for the student with email {student.Email}"
                    };

                // (5/7) => Getting the user whose account just got created in the above step with the given email.
                logger.LogInfo($"(5/7) => Getting the user whose account just got created in the above step with the given email.");
                var newUser = await userManagement.GetUserByEmailAsync(student.Email);
                if (newUser == null)
                    return new ServiceResponse()
                    {
                        Success = false,
                        Message = $"Not able to find the user with email {student.Email}"
                    };

                // (6/7) => Assigning the new student with the student role within the system.
                logger.LogInfo($"(6/7) => Assigning the new student with the student role within the system.");
                var roleResult = await roleManagement.AddUserToRoleAsync(newUser, "Student");
                if (!roleResult)
                    return new ServiceResponse()
                    {
                        Success = false,
                        Message = $"Error while assigning role to the user with email {student.Email}"
                    };

                // (7/7) => Preparing the new student entity and perge new student record into the student store.
                logger.LogInfo($"(7/7) => Preparing the new student entity and perge new student record into the student store.");
               
                // Check if the string ID is a valid GUID before assignment
                if (Guid.TryParse(newUser.Id, out Guid userIdGuid))
                {
                    ((IStudentHasInternalIds)student).UserId = userIdGuid;
                }
                else
                {
                    // Handle error: Identity ID was not a valid GUID format
                }
                //((IStudentHasInternalIds)student).UserId = newUser.Id;
                ((IStudentHasInternalIds)student).StudentCode = await GenerateNewStudentCode();
                var mappedStudent = mapper.Map<Student>(student);

                await studentRepository.AddAsync(mappedStudent);
                var result  = await unitOfWork.CommitAsync(cancellationToken);

                if (result <= 0)
                    return new ServiceResponse()
                    {
                        Success = false,
                        Message = $"Student is not created with the email {student.Email}"
                    };
                else
                    return new ServiceResponse()
                    {
                        Success = true,
                        Message = $"New student is created with the email: {student.Email}"
                    };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while onboarding a new student into the system");

                return new ServiceResponse()
                {
                    Success = false,
                    Message = $"Error while onboarding a new student into the system. " +
                    $"Kindly check the log for more details."
                };
            }
        }

        /// <summary>
        /// This is to generate a new student code within the system.
        /// </summary>
        /// <returns></returns>
        private async Task<string> GenerateNewStudentCode()
        {
            return await studentCodeGeneratorService
                .GenerateNewStudentCodeAsync(DateTime.UtcNow);
        }

        /// <summary>
        /// This is to generate a new temproray password for the student.
        /// This password is needed when the student first logs into the system.
        /// </summary>
        /// <returns></returns>
        private string GenerateNewPassword()
        {
            return passwordGeneratorService
                .GenerateSecurePassword();
        }
    }
}