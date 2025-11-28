using Microsoft.EntityFrameworkCore.Storage;
using SMS.Domain.Entities.Identity;

namespace SMS.Application.Services.Identity
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
    }
}
