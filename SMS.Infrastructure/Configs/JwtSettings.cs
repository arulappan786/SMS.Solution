namespace SMS.Infrastructure.Configs
{
    public class JwtSettings
    {
        public const string SettingsKey = "JwtSettings";

        public required string SecretKey { get; init; }

        public required string ValidIssuer { get; init; }

        public required string ValidAudience { get; init; }

        public required int TokenDurationInMinutes { get; init; }

        public required int RefreshTokenDurationInDays { get; init; }


    }
}
