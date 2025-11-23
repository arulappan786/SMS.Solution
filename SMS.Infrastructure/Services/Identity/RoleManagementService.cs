using Microsoft.AspNetCore.Identity;
using SMS.Application.Services.Interfaces.Identity;
using SMS.Domain.Entities.Identity;

namespace SMS.Infrastructure.Services.Identity
{
    public class RoleManagementService(UserManager<AppUser> userManager) : IRoleManagementService
    {
        public async Task<bool> AddUserToRoleAsync(AppUser user, string roleName)
        {
            return (await userManager.AddToRoleAsync(user, roleName)).Succeeded;
        }

        public async Task<string?> GetUserRoleAsync(string userEmail)
        {
            var user = await userManager.FindByEmailAsync(userEmail);
            return (await userManager.GetRolesAsync(user!)).FirstOrDefault();
        }
    }
}
