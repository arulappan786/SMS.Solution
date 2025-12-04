using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SMS.Application.DTOs.Identity;
using SMS.Application.Services.Identity;
using SMS.Application.Services.Logging;
using SMS.Domain.Entities.Identity;
using SMS.Infrastructure.Configs;

namespace SMS.Infrastructure.Services.Identity
{
    public class IdentityService(
        UserManager<AppUser> userManager,
        ITokenService tokenService,
        IUserManagementService userManagementService, // Added for delegated persistence
        IOptions<JwtSettings> options,
        IAppLogger<IdentityService> logger) : IIdentityService
    {
        private readonly JwtSettings _jwtSettings = options.Value; // Accessing .Value here is common, though IOptions<T> is preferred. Sticking to your original for minimum change.

        public async Task<(bool Success, string Message, LoggedInUserDto? UserDto)> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken)
        {
            logger.LogInfo($"Starting {nameof(LoginAsync)} for email: {email}");

            // --- 1. Retrieve the User (Including null/generic error check) ---
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                logger.LogWarning($"Login failed: User not found for email: {email}");
                return (false, "Invalid credentials.", null);
            }

            // --- 2. Security Checks ---
            var passwordCheck = await userManager.CheckPasswordAsync(user, password);
            if (!passwordCheck)
            {
                logger.LogWarning($"Login failed: Invalid password for user ID: {user.Id}");
                return (false, "Invalid credentials.", null);
            }

            if (await userManager.IsLockedOutAsync(user))
            {
                logger.LogWarning($"Login failed: User account is locked out for user ID: {user.Id}");
                return (false, "Account is locked out. Please try again later.", null);
            }

            // --- 3. Generate Tokens and Roles ---
            var roles = await userManager.GetRolesAsync(user);

            // Access Token details
            int accessTokenLifetimeSeconds = _jwtSettings.TokenDurationInMinutes * 60;
            var accessToken = tokenService.GenerateAccessToken(user, roles.ToList());

            // Refresh Token details
            var refreshToken = tokenService.GenerateRefreshToken();
            const int refreshTokenLifetimeDays = 7; // Define constant lifespan for refresh token
            var expiryTime = DateTime.UtcNow.AddDays(refreshTokenLifetimeDays);

            // --- 4. Delegate Refresh Token Persistence to User Management Service ---
            var (saveSuccess, saveMessage) = await userManagementService.UpdateRefreshTokenAsync(
                Guid.Parse(user.Id),
                refreshToken,
                expiryTime,
                cancellationToken);

            if (!saveSuccess)
            {
                logger.LogError($"Login failed: Refresh token save failed for user {user.Id}. Reason: {saveMessage}");
                return (false, "Login failed due to a critical server error.", null);
            }
            
            logger.LogInfo($"Refresh token successfully delegated and saved for user {user.Id}.");

            // --- 5. Return DTO ---
            var userDto = new LoggedInUserDto(
                UserId: Guid.Parse(user.Id),
                UserName: user.UserName!,
                EmailAddress: user.Email!,
                Roles: roles.ToList(),
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                ExpiresInSeconds: accessTokenLifetimeSeconds
            );

            return (true, "Login successful.", userDto);
        }
    }
}