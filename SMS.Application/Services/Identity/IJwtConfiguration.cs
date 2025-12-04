namespace SMS.Application.Services.Identity
{
    public interface IJwtConfiguration
    {
        int TokenDurationInMinutes { get; }
        int RefreshTokenDurationInDays { get; }
    }
}
