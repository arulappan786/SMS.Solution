using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SMS.Application.Services.Identity;
using SMS.Domain.Entities.Identity;
using SMS.Infrastructure.Persistance.Context;

namespace SMS.Infrastructure.Services.Identity
{
    public class UserManagementService(UserManager<AppUser> userManager,
                                       AppDbContext dbContext) : IUserManagementService
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
    }
}
