using SMS.Domain.Entities.Identity;
using System.Security.Claims;

namespace SMS.Application.Services.Identity
{
    public interface ITokenService
    {
        string GenerateAccessToken(AppUser user, IList<string> roles);

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
