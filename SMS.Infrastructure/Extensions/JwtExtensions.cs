using Microsoft.IdentityModel.Tokens;
using SMS.Domain.Entities.Identity;
using SMS.Infrastructure.Configs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SMS.Infrastructure.Extensions
{
    public static class JWTExtensions
    {
        public static SymmetricSecurityKey GetSymmetricSecurityKey(this JwtSettings settings)
        {
            var keyBytes = Encoding.UTF8.GetBytes(settings.SecretKey);
            return new SymmetricSecurityKey(keyBytes);
        }

        public static SigningCredentials GetSigningCredentials(this JwtSettings settings)
        {
            var securityKey = settings.GetSymmetricSecurityKey();
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }

        public static List<Claim> GetClaims(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                // Sub (Subject): A unique identifier for the user (often the User ID)
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                // Jti (JWT ID): A unique identifier for the token itself (good practice)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Email
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                // Custom claim for the display name/username
                new Claim(ClaimTypes.Name, user.UserName!),
            };

            // Add roles as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}
