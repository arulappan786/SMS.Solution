using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SMS.Application.Services.Identity;
using SMS.Domain.Entities.Identity;
using SMS.Infrastructure.Configs;
using SMS.Infrastructure.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SMS.Infrastructure.Services.Identity
{
    public class TokenService(IOptions<JwtSettings> options) : ITokenService
    {
        private readonly JwtSettings _jwtSettings = options.Value;

        public string GenerateAccessToken(AppUser user, IList<string> roles)
        {
            var securityKey = options.Value.GetSymmetricSecurityKey();
            var signingCredentials = options.Value.GetSigningCredentials();
            var claims = JWTExtensions.GetClaims(user, roles);

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

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            // --- 1. Define Token Validation Parameters ---
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _jwtSettings.ValidAudience,

                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.ValidIssuer,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _jwtSettings.GetSymmetricSecurityKey(),

                // CRUCIAL: Set ValidateLifetime to false to allow validation of expired tokens
                ValidateLifetime = false,

                // Optional: Ensure clock skew is handled during validation
                ClockSkew = TimeSpan.Zero
            };

            // --- 2. Create Handler and Principal ---
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            // This method attempts to read and validate the token signature and claims 
            // based on the parameters above. It will throw an exception if the signature is invalid.
            var principal = tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out securityToken);

            // --- 3. Final Security Check: Ensure token is a JWT and uses the correct algorithm ---
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token format or algorithm.");
            }

            return principal;
        }
    }
}
