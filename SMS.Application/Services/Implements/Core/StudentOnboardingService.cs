using Microsoft.EntityFrameworkCore.Storage;
using SMS.Application.CQRS.Core.Students.Commands;
using SMS.Application.Services.Interfaces.Core;
using SMS.Application.Services.Interfaces.Identity;
using SMS.Application.Services.Interfaces.Logging;
using SMS.Domain.Entities.Identity;
using SMS.Domain.Interfaces.Repositories;

namespace SMS.Application.Services.Implements.Core
{
    // This service handles the critical Identity-level data orchestration (User and Role creation).
    public class StudentOnboardingService : IStudentOnboardingService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserManagementService _userManagement;
        private readonly IRoleManagementService _roleManagement;
        private readonly IUnitOfWork _unitOfWork; // Correctly used for transaction control
        private readonly IAppLogger<StudentOnboardingService> _logger;

        public StudentOnboardingService(
            IStudentRepository studentRepository,
            IUserManagementService userManagement,
            IRoleManagementService roleManagement,
            IUnitOfWork unitOfWork,
            IAppLogger<StudentOnboardingService> logger)
        {
            _studentRepository = studentRepository;
            _userManagement = userManagement;
            _roleManagement = roleManagement;
            _unitOfWork = unitOfWork; // Initialized here
            _logger = logger;
        }

        public async Task<bool> IsUniqueAsync(string email, CancellationToken cancellationToken)
        {
            // 1. Check Student Repository (Business Entity Store)
            var studentByEmail = await _studentRepository.GetByEmailAsync(email, cancellationToken);
            if (studentByEmail != null) return false;

            // 2. Check User Management (Identity Store)
            var userByStudentMail = await _userManagement.GetUserByEmailAsync(email);
            if (userByStudentMail != null) return false;

            return true;
        }

        public async Task<AppUser> CreateUserAndAssignRoleAsync(
            CreateStudentCommand studentCommand,
            string password,
            CancellationToken cancellationToken)
        {
            IDbContextTransaction? transaction = null;

            // START TRANSACTION (ACID Boundary for Identity Operations)
            try
            {
                // *** KEY CHANGE: Using IUnitOfWork to start the transaction ***
                transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // 1. Create the AppUser entity
                var newUser = new AppUser
                {
                    Email = studentCommand.Email,
                    PasswordHash = password, // Temp password passed from handler
                    DisplayName = studentCommand.FullName.ToString(),
                    UserName = studentCommand.Email
                };

                // 2. Create the AppUser (Must be refactored to use the explicit transaction)
                // Assuming IUserManagementService has a method that accepts and uses the transaction handle.
                var createResult = await _userManagement.CreateUserWithTransactionAsync(newUser, password, transaction);

                if (!createResult)
                {
                    throw new InvalidOperationException($"User creation failed for email: {studentCommand.Email}.");
                }

                // We need to fetch the newly created user for the Role assignment and mapping
                var user = await _userManagement.GetUserByEmailAsync(studentCommand.Email);

                if (user == null)
                {
                    // This failure will trigger the catch block and Rollback
                    throw new InvalidOperationException("Could not retrieve newly created user for role assignment.");
                }

                // 3. Assign the Student Role
                // Assuming IRoleManagementService has a method that accepts and uses the transaction handle.
                var roleResult = await _roleManagement.AddUserToRoleWithTransactionAsync(user, "Student", transaction);

                if (!roleResult)
                {
                    throw new InvalidOperationException($"Role 'Student' assignment failed for user: {studentCommand.Email}.");
                }

                // COMMIT: Note: We DO NOT COMMIT HERE. 
                // We leave the transaction pending and return the user. 
                // The main orchestrator (StudentService) will COMMIT the single transaction 
                // after adding the Student record to the main repository.

                return user;
            }
            catch (Exception ex)
            {
                // ROLLBACK: If anything failed in the persistent store, use IUnitOfWork to roll back.
                if (transaction != null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                }
                _logger.LogError(ex, $"Transaction rolled back during user creation for {studentCommand.Email}.");
                throw; // Re-throw the exception to be caught by the main orchestrator
            }
        }
    }
}