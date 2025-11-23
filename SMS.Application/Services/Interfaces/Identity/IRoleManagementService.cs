using SMS.Domain.Entities.Identity;

namespace SMS.Application.Services.Interfaces.Identity
{
    public interface IRoleManagementService
    {
        Task<string?> GetUserRoleAsync(string userEmail);

        Task<bool> AddUserToRoleAsync(AppUser user, string roleName);
    }
}
