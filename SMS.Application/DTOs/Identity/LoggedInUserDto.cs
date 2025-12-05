namespace SMS.Application.DTOs.Identity
{
    // Identity
    // Tokens
    public record LoggedInUserDto(Guid UserId, string UserName, string Email, List<string> Roles, string AccessToken, string RefreshToken, int ExpiresInSeconds);
}
