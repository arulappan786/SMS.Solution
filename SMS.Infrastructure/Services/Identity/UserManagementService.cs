using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SMS.Application.Services.Identity;
using SMS.Domain.Entities.Identity;
using SMS.Infrastructure.Persistance.Context;
using System.Security.Claims;

namespace SMS.Infrastructure.Services.Identity
{
    public class UserManagementService(IRoleManagementService roleManagement,
        UserManager<AppUser> userManager,
        AppDbContext dbContext) : IUserManagementService
    {



        //public async Task<bool> CreateUserAsync(AppUser user)
        //{
        //    AppUser? _user = await GetUserByEmailAsync(user.Email!);
        //    if (_user != null) return false;

        //    var result = await userManager.CreateAsync(user, user.PasswordHash!);

        //    return result.Succeeded;
        //}

        //public async Task<IEnumerable<AppUser>?> GetAllUsersAsync()
        //{
        //    return await dbContext.Users.ToListAsync();
        //}

        //public async Task<List<Claim>> GetClaimsByEmailAsync(string email)
        //{
        //    var _user = await GetUserByEmailAsync(email);
        //    string? userRole = await roleManagement.GetUserRoleAsync(_user!.Email!);

        //    return new List<Claim> {
        //        new Claim("DisplayName", _user!.DisplayName!.ToString()),
        //        new Claim(ClaimTypes.NameIdentifier, _user!.Id!.ToString()),
        //        new Claim(ClaimTypes.Email, _user!.Email!),
        //        new Claim(ClaimTypes.Role, userRole!)
        //    };
        //}

        //public async Task<AppUser?> GetUserByEmailAsync(string email)
        //{
        //    return await userManager.FindByEmailAsync(email);
        //}

        //public async Task<AppUser> GetUserByIdAsync(string userId)
        //{
        //    var user = await userManager.FindByIdAsync(userId);
        //    return user!;
        //}

        //public async Task<AppUser> GetUserByNameAsync(string userName)
        //{
        //    var user = await userManager.FindByNameAsync(userName);
        //    return user!;
        //}

        //public async Task<bool> LoginUserAsync(AppUser user, string password)
        //{
        //    var _user = await GetUserByEmailAsync(user!.Email!);
        //    if (_user is null) return false;

        //    string? roleName = await roleManagement.GetUserRoleAsync(user!.Email!);
        //    if (string.IsNullOrEmpty(roleName)) return false;

        //    var result = await userManager.CheckPasswordAsync(user, password);

        //    return result;
        //}

        //public async Task<bool> RemoveUserByEmailAsync(string email)
        //{
        //    var _user = await dbContext.Users.FirstOrDefaultAsync(_ => _.Email == email);
        //    dbContext.Users.Remove(_user!);
        //    var result = await dbContext.SaveChangesAsync();
        //    return result > 0;
        //}
       
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
    }
}
