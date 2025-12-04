using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SMS.Application.DTOs.Identity;
using SMS.Application.Services.Identity;
using SMS.Application.Services.Logging;
using SMS.Domain.Entities.Identity;
using SMS.Infrastructure.Persistance.Context;

namespace SMS.Infrastructure.Services.Identity
{
    public class UserManagementService(UserManager<AppUser> userManager,
                                       AppDbContext dbContext, IAppLogger<UserManagementService> logger) : IUserManagementService
    {

        public async Task<bool> CreateUserWithTransactionAsync(AppUser user, string password, IDbContextTransaction transaction)
        {
            // IMPORTANT: Attach the transaction handle to the DbContext used by the UserManager
            // This ensures that the Identity operation (which calls SaveChanges internally) uses the external transaction.
            await dbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

            var result = await userManager.CreateAsync(user, password);

            // The transaction is NOT committed here; it remains controlled by the caller (StudentOnboardingService)

            return result.Succeeded;
        }

        public async Task<IdentityResult> DeleteUserAsync(AppUser user)
        {
            if (user == null)
            {
                // Return a failed result if the user object is null
                return IdentityResult.Failed(new IdentityError { Description = "Cannot delete a null user object." });
            }

            // The core deletion call using the Identity framework's UserManager
            return await userManager.DeleteAsync(user);
        }

        public async Task<AppUser?> GetUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Updates properties of a provided AppUser, including linking the Student Profile ID.
        /// </summary>
        /// <param name="user">The AppUser entity retrieved from the identity store.</param>
        /// <param name="studentProfileId">The Student Profile ID (GUID) to link to the user.</param>
        /// <returns>An IdentityResult indicating success or failure.</returns>
        public async Task<IdentityResult> LinkStudentProfileToUserAsync(AppUser user, Guid studentProfileId, CancellationToken cancellationToken)
        {
            // 1. Validation Check
            if (user == null)
            {
                // Should not happen if the calling code retrieved it properly, but good defensive check
                return IdentityResult.Failed(new IdentityError { Description = "Provided AppUser object is null." });
            }

            // 2. Apply Custom Property Update
            // Check if the ID is different to avoid unnecessary database writes
            if (user.StudentProfileId != studentProfileId)
            {
                user.StudentProfileId = studentProfileId;
            }

            // 3. Persist all changes to the user store
            // This is the essential step. It tells the UserManager to save the changes 
            // to the underlying IdentityDbContext.
            return await userManager.UpdateAsync(user);
        }

        // Inside UserManagementService implementation

        public async Task<(bool Success, string Message)> UpdateRefreshTokenAsync(Guid userId, string? refreshToken, DateTime? expiryTime, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                logger.LogWarning($"Token update failed: User with ID {userId} not found.");
                return (false, "User not found.");
            }

            // 1. Apply the token data to the entity
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiryTime;

            // 2. Persist the changes
            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                logger.LogInfo($"Refresh token updated successfully for user {userId}.");
                return (true, "Refresh token saved.");
            }
            else
            {
                logger.LogError($"Failed to save refresh token for user {userId}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return (false, "Failed to save refresh token data.");
            }
        }        
    }
}
