using Microsoft.Extensions.Options;
using SMS.Application.Services.Identity;
using SMS.Infrastructure.Configs;

namespace SMS.Infrastructure.Services.Identity
{
    public class JwtConfigurationAccessor : IJwtConfiguration
    {
        public int TokenDurationInMinutes => _jwtSettings.TokenDurationInMinutes;

        public int RefreshTokenDurationInDays => _jwtSettings.RefreshTokenDurationInDays;

        private readonly JwtSettings _jwtSettings;

        // Inject the infrastructure configuration and extract the value
        public JwtConfigurationAccessor(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }
    }
}
