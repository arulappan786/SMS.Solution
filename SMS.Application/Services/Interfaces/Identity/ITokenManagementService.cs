using System.Security.Claims;

namespace SMS.Application.Services.Interfaces.Identity
{
    public interface ITokenManagementService
    {
        string GenerateToken(List<Claim> claims);
        //string GenerateRefreshToken();
        List<Claim> GetUserClaimsFromTokenAsync(string token);
        //Task<bool> ValidateRefreshTokenAsync(string refreshToken);
        //Task<string> GetUserIdByRefreshTokenAsync(string refreshToken);
        //Task<bool> AddUserRefreshTokenAsync(string userId, string refreshToken);
        //Task<bool> UpdateUserRefreshTokenAsync(string userId, string refreshToken);
    }
}
