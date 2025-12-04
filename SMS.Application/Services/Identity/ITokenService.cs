using SMS.Domain.Entities.Identity;

namespace SMS.Application.Services.Identity
{
    public interface ITokenService
    {
        string GenerateAccessToken(AppUser user, IList<string> roles);

        string GenerateRefreshToken();
    }
}
