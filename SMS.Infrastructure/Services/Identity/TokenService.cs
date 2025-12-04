using Microsoft.Extensions.Options;
using SMS.Application.Services.Identity;
using SMS.Domain.Entities.Identity;
using SMS.Infrastructure.Configs;
using SMS.Infrastructure.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace SMS.Infrastructure.Services.Identity
{
    public class TokenService(IUserManagementService userManagementService, IOptions<JwtSettings> options) : ITokenService
    {
        private readonly JwtSettings _jwtSettings = options.Value;

        public string GenerateAccessToken(AppUser user, IList<string> roles)
        {
            var securityKey = options.Value.GetSymmetricSecurityKey();
            var signingCredentials = options.Value.GetSigningCredentials();
            var claims = JwtExtensions.GetClaims(user, roles);

            var securityToken = new JwtSecurityToken(
                issuer: _jwtSettings.ValidIssuer,
                audience: _jwtSettings.ValidAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenDurationInMinutes),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        public string GenerateRefreshToken()
        {
            // A 32-byte array (256 bits) is generally a secure length for a token.
            var randomNumber = new byte[32];

            // Fills the byte array with a cryptographically strong sequence of random values.
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                // We use URL-safe Base64 encoding to convert the bytes into a string 
                // that can be safely transmitted in URLs and JSON.
                return Convert.ToBase64String(randomNumber);
            }
        }

    }
}
