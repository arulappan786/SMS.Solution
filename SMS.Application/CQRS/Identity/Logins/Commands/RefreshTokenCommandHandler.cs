using MediatR;
using Microsoft.AspNetCore.Identity;
using SMS.Application.DTOs.Identity;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Identity;
using SMS.Application.Services.Logging;
using SMS.Domain.Entities.Identity;
using System.Security.Claims;

namespace SMS.Application.CQRS.Identity.Logins.Commands
{
    public class RefreshTokenCommandHandler(ITokenService tokenService,
                                            UserManager<AppUser> userManager,
                                            IUserManagementService userManagementService,
                                            IJwtConfiguration jwtConfig,
                                            IAppLogger<RefreshTokenCommandHandler> logger) 
        : IRequestHandler<RefreshTokenCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // --- 1. Get Claims from Expired Access Token ---
            ClaimsPrincipal principal;
            try
            {
                principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Token refresh failed: Invalid or malformed expired AccessToken.");
                return ServiceResponse.Failure("Invalid token submission.");
            }

            var userIdString = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
            {
                return ServiceResponse.Failure("Invalid token claims.");
            }

            // --- 2. Retrieve User and Validate Refresh Token ---
            var user = await userManager.FindByIdAsync(userIdString);

            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                logger.LogWarning($"Token refresh failed for user {user?.Id}: Invalid token or token expired/revoked.");
                // OPTIONAL SECURITY STEP: If a bad token is submitted, revoke all tokens for this user immediately.

                return ServiceResponse.Failure("Invalid refresh token or session expired.");
            }

            // --- 3. Generate New Tokens ---
            var roles = await userManager.GetRolesAsync(user);

            var newAccessToken = tokenService.GenerateAccessToken(user, roles.ToList());
            var newRefreshToken = tokenService.GenerateRefreshToken();
            var newExpiryTime = DateTime.UtcNow.AddDays(jwtConfig.RefreshTokenDurationInDays);

            // --- 4. Delegate Persistence of New Refresh Token ---
            var (saveSuccess, saveMessage) = await userManagementService.UpdateRefreshTokenAsync(
                Guid.Parse(user.Id),
                newRefreshToken,
                newExpiryTime,
                cancellationToken);

            if (!saveSuccess)
            {
                logger.LogError($"Token refresh failed: Unable to save new refresh token. Details: {saveMessage}");
                return ServiceResponse.Failure("Token renewal failed due to internal error.");
            }

            // --- 5. Return New Token Pair ---
            var userDto = new LoggedInUserDto(
                UserId: Guid.Parse(user.Id),
                UserName: user.UserName!,
                Email: user.Email!,
                Roles: roles.ToList(),
                AccessToken: newAccessToken,
                RefreshToken: newRefreshToken,
                ExpiresInSeconds: (int)jwtConfig.TokenDurationInMinutes * 60 // Assuming you can access JwtSettings
            );

            return ServiceResponse.Success("Token successfully renewed.", data: userDto);
        }
    }
}
