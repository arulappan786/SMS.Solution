using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SMS.Application.Services.Interfaces.Identity;
using SMS.Domain.Entities.Identity;
using SMS.Infrastructure.Persistance.Context;

namespace SMS.Infrastructure.Services.Identity
{
    public class RoleManagementService(UserManager<AppUser> userManager, AppDbContext dbContext) : IRoleManagementService
    {
        public Task<bool> AddUserToRoleAsync(AppUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a user to the specified role using an existing transaction.
        /// </summary>
        /// <param name="user">The user entity.</param>
        /// <param name="role">The role name to assign.</param>
        /// <param name="transaction">The active database transaction to include this operation in.</param>
        /// <returns>True if role assignment succeeded, false otherwise.</returns>
        public async Task<bool> AddUserToRoleWithTransactionAsync(AppUser user, string role, IDbContextTransaction transaction)
        {
            // Attach the transaction handle to the DbContext used by the UserManager
            await dbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

            var result = await userManager.AddToRoleAsync(user, role);

            // The transaction is NOT committed here; it remains controlled by the caller.
            // Note: We don't need to detach the transaction here as the overall transaction scope 
            // is managed by the IUnitOfWork wrapper, which will commit/rollback and dispose of the transaction.

            return result.Succeeded;
        }        

        public async Task<string?> GetUserRoleAsync(string userEmail)
        {
            var user = await userManager.FindByEmailAsync(userEmail);
            return (await userManager.GetRolesAsync(user!)).FirstOrDefault();
        }
    }
}
