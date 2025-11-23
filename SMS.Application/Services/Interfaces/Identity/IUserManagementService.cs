using SMS.Domain.Entities.Identity;
using System.Security.Claims;

namespace SMS.Application.Services.Interfaces.Identity
{
    public interface IUserManagementService
    {
        Task<bool> CreateUserAsync(AppUser user);

        Task<bool> LoginUserAsync(AppUser user, string password);

        Task<AppUser?> GetUserByEmailAsync(string email);

        Task<AppUser> GetUserByIdAsync(string userId);

        Task<AppUser> GetUserByNameAsync(string userName);

        Task<IEnumerable<AppUser>?> GetAllUsersAsync();

        Task<bool> RemoveUserByEmailAsync(string email);

        Task<List<Claim>> GetClaimsByEmailAsync(string email);
    }
}
