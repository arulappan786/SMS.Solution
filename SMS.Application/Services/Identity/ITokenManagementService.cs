using System.Security.Claims;

namespace SMS.Application.Services.Identity
{
    public interface ITokenManagementService
    {
        string GenerateToken(List<Claim> claims);
        List<Claim> GetUserClaimsFromTokenAsync(string token);
    }
}
