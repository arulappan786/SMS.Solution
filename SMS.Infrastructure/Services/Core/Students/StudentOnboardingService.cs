using Microsoft.EntityFrameworkCore.Storage;
using SMS.Application.CQRS.Core.Students.Commands.Create;
using SMS.Application.Services.Core.Students;
using SMS.Application.Services.Identity;
using SMS.Application.Services.Logging;
using SMS.Domain.Entities.Identity;
using SMS.Domain.Enums;
using SMS.Domain.Interfaces.Repositories.Common;
using SMS.Domain.Interfaces.Repositories.Core;

namespace SMS.Infrastructure.Services.Core.Students
{
    public class StudentOnboardingService(
        IStudentRepository studentRepository,
        IUserManagementService userManagement,
        IRoleManagementService roleManagement,
        IUnitOfWork unitOfWork,
        IAppLogger<StudentOnboardingService> logger) : IStudentOnboardingService
    {
        
        /// <summary>
        /// To check the uniqueness of the student using his/her email id
        /// This checks on both the student store and user store
        /// email id is the unique identifier for checking the student uniqueness within the system.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> IsUniqueAsync(string email, CancellationToken cancellationToken)
        {
            var studentByEmail = await studentRepository.GetByEmailAsync(email, cancellationToken);
            if (studentByEmail != null) return false;

            var userByStudentMail = await userManagement.GetUserByEmailAsync(email);
            if (userByStudentMail != null) return false;

            return true;
        }

        /// <summary>
        /// To create a new user and assign him with the student role.
        /// </summary>
        /// <param name="studentCommand"></param>
        /// <param name="password"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<AppUser> CreateUserAndAssignRoleAsync(
            CreateStudentCommand studentCommand,
            string password,
            CancellationToken cancellationToken)
        {
            IDbContextTransaction? transaction = null;

            try
            {
                transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

                var newUser = new AppUser
                {
                    Email = studentCommand.Email,
                    PasswordHash = password,
                    DisplayName = studentCommand.FullName.ToString(),
                    UserName = studentCommand.Email,                    
                };

                var createResult = await userManagement.CreateUserWithTransactionAsync(newUser, password, transaction);

                if (!createResult)
                {
                    throw new InvalidOperationException($"User creation failed for email: {studentCommand.Email}.");
                }

                var user = await userManagement.GetUserByEmailAsync(studentCommand.Email);

                if (user == null)
                {
                    throw new InvalidOperationException("Could not retrieve newly created user for role assignment.");
                }

                var roleResult = await roleManagement.AddUserToRoleWithTransactionAsync(user, AppRole.Student.ToString(), transaction);

                if (!roleResult)
                {
                    throw new InvalidOperationException($"Role 'Student' assignment failed for user: {studentCommand.Email}.");
                }                

                return user;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await unitOfWork.RollbackAsync(cancellationToken);
                }
                logger.LogError(ex, $"Transaction rolled back during user creation for {studentCommand.Email}.");
                throw;
            }
        }

        public async Task<bool> LinkStudentProfileToUserAsync(AppUser user, Guid studentProfileId, CancellationToken cancellationToken)
        {
           var result = await userManagement.LinkStudentProfileToUserAsync(user, studentProfileId, cancellationToken);
            return result.Succeeded;
        }
    }
}