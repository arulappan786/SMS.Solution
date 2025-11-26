using AutoMapper;
using FluentValidation;
using SMS.Application.CQRS.Core.Students.Commands;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Interfaces.Common;
using SMS.Application.Services.Interfaces.Core;
using SMS.Application.Services.Interfaces.Identity;
using SMS.Application.Validations;
using SMS.Domain.Entities.Core;
using SMS.Domain.Entities.Identity;
using SMS.Domain.Interfaces.Repositories;

namespace SMS.Application.Services.Implements.Core
{
    public class StudentService(IStudentRepository studentRepository,
                                IUserManagementService userManagement,
                                IRoleManagementService roleManagement,
                                IPasswordGeneratorService passwordGeneratorService,
                                IStudentCodeGeneratorService studentCodeGeneratorService,
                                IValidationService validationService,
                                IValidator<CreateStudentCommand> validator,
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
                // 1. User Input Validation.
                var validationResult = await validationService.ValidateAsync(student, validator);
                if (!validationResult.Success)
                    return validationResult;

                // 2. Check if the same student with the same email exists already in the student store.
                var studentByEmail = await studentRepository.GetByEmailAsync(student.Email);
                if (studentByEmail != null)
                    return new ServiceResponse()
                    {
                        Success = false,
                        Message = $"Student with the same email {student.Email} exists already."
                    };

                // 3. Check if the same student with the same email exists already in the user store.
                var userByStudentMail = await userManagement.GetUserByEmailAsync(student.Email);
                if (userByStudentMail != null)
                    return new ServiceResponse()
                    {
                        Success = false,
                        Message = $"Student with the same email {student.Email} exists already. " +
                        $"Please check for the user existence with this email {student.Email}"
                    };

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

                var newUser = await userManagement.GetUserByEmailAsync(student.Email);
                if (newUser == null)
                    return new ServiceResponse()
                    {
                        Success = false,
                        Message = $"Not able to find the user with email {student.Email}"
                    };

                var roleResult = await roleManagement.AddUserToRoleAsync(newUser, "Student");
                if (!roleResult)
                    return new ServiceResponse()
                    {
                        Success = false,
                        Message = $"Error while assigning role to the user with email {student.Email}"
                    };

                ((IStudentHasInternalIds)student).UserId = newUser!.Id;
                ((IStudentHasInternalIds)student).StudentCode = await GenerateNewStudentCode();

                var mappedStudent = mapper.Map<Student>(student);
                var resultStudentAdd = await studentRepository.AddAsync(mappedStudent);

                if (!resultStudentAdd)
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

                return new ServiceResponse()
                {
                    Success = false,
                    Message = $"Error while creting the student with email: {student.Email}. Error : {ex.InnerException}"
                };
            }
        }

        private async Task<string> GenerateNewStudentCode()
        {
            return await studentCodeGeneratorService.GenerateNewStudentCodeAsync(DateTime.UtcNow);
        }

        private string GenerateNewPassword()
        {
            return passwordGeneratorService.GenerateSecurePassword();
        }
    }
}