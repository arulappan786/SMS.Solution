namespace SMS.Application.DTOs.Identity
{
    public record LoginDto(string EmailAddress, string Password, bool RememberMe);
}
