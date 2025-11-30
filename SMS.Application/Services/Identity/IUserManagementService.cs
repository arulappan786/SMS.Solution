using Microsoft.AspNetCore.Identity;
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

        Task<IdentityResult> LinkStudentProfileToUserAsync(AppUser user, Guid studentProfileId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the user from the identity store.
        /// </summary>
        /// <param name="user">The AppUser entity to be deleted.</param>
        /// <returns>IdentityResult indicating success or failure of the deletion.</returns>
        Task<IdentityResult> DeleteUserAsync(AppUser user);
    }
}
