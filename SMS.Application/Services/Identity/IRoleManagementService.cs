using Microsoft.EntityFrameworkCore.Storage;
using SMS.Domain.Entities.Identity;

namespace SMS.Application.Services.Identity
{
    public interface IRoleManagementService
    {
        Task<string?> GetUserRoleAsync(string userEmail);
        Task<bool> AddUserToRoleAsync(AppUser user, string roleName);
        Task<bool> AddUserToRoleWithTransactionAsync(AppUser user, string role, IDbContextTransaction transaction);
    }
}
