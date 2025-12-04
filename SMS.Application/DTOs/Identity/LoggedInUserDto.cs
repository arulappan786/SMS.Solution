namespace SMS.Application.DTOs.Identity
{
    // Identity
    // Tokens
    public record LoggedInUserDto(Guid UserId, string UserName, string EmailAddress, List<string> Roles, string AccessToken, string RefreshToken, int ExpiresInSeconds);
}
