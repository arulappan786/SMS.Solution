using Microsoft.EntityFrameworkCore.Storage;
using SMS.Domain.Entities.Identity;

namespace SMS.Application.Services.Interfaces.Identity
{
    public interface IUserManagementService
    {
        Task<AppUser?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Creates a new application user with the specified credentials, using an existing transaction.
        /// </summary>
        /// <param name="user">The user entity to create.</param>
        /// <param name="password">The plain text password for the user.</param>
        /// <param name="transaction">The active database transaction to include this operation in.</param>
        /// <returns>True if creation succeeded, false otherwise.</returns>
        Task<bool> CreateUserWithTransactionAsync(AppUser user, string password, IDbContextTransaction transaction);


        // Example: The original simple method (optional, depends on use case):
        // Task<bool> CreateUserAsync(AppUser user); 

        //Task<bool> CreateUserAsync(AppUser user);

        //Task<bool> LoginUserAsync(AppUser user, string password);

        //Task<AppUser?> GetUserByEmailAsync(string email);

        //Task<AppUser> GetUserByIdAsync(string userId);

        //Task<AppUser> GetUserByNameAsync(string userName);

        //Task<IEnumerable<AppUser>?> GetAllUsersAsync();

        //Task<bool> RemoveUserByEmailAsync(string email);

        //Task<List<Claim>> GetClaimsByEmailAsync(string email);
    }
}
