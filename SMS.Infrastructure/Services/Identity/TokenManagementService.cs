using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SMS.Application.Services.Interfaces.Identity;
using SMS.Infrastructure.Persistance.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SMS.Infrastructure.Services.Identity
{
    public class TokenManagementService(AppDbContext dbContext, IConfiguration config) : ITokenManagementService
    {
        //public async Task<bool> AddUserRefreshTokenAsync(string userId, string refreshToken)
        //{
        //    dbContext.RefreshToken.Add(new RefreshToken()
        //    {
        //        UserId = userId,
        //        Token = refreshToken
        //    });

        //    var result = await dbContext.SaveChangesAsync();

        //    return (result > 0);
        //}

        public string GenerateToken(List<Claim> claims)
        {
            var secretKey = config["JWT:SecretKey"];
            var issuer = config["JWT:ValidIssuer"];
            var audience = config["JWT:ValidAudience"];
            var duration = config["JWT:TokenDurationInMinutes"];

            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience) || string.IsNullOrWhiteSpace(duration))
            {
                throw new InvalidOperationException("JWT configuration is missing or invalid.");
            }

            var Seckey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(Seckey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(Convert.ToInt32(duration));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var base64String = Convert.ToBase64String(randomBytes);
            //return WebUtility.UrlEncode(base64String);
            return base64String;
        }

        public List<Claim> GetUserClaimsFromTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            if (jwtToken == null) return [];
            return jwtToken.Claims.ToList();
        }

        //public async Task<string> GetUserIdByRefreshTokenAsync(string refreshToken)
        //{
        //    return (await dbContext.RefreshToken.FirstOrDefaultAsync(_ => _.Token == refreshToken))!.UserId!;
        //}

        //public async Task<bool> UpdateUserRefreshTokenAsync(string userId, string refreshToken)
        //{
        //    var _refreshToken = await dbContext.RefreshToken
        //        .FirstOrDefaultAsync(_ => _.UserId.Equals(userId));
        //    if (_refreshToken == null) return false;

        //    _refreshToken.Token = refreshToken;

        //    var result = await dbContext.SaveChangesAsync();

        //    return (result > 0);
        //}

        //public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        //{
        //    var _refreshToken = await dbContext.RefreshToken
        //        .FirstOrDefaultAsync(_ => _.Token.Equals(refreshToken));
        //    return (_refreshToken is not null);
        //}
    }
}
