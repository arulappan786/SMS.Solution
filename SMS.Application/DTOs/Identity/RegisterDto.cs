namespace SMS.Application.DTOs.Identity
{
    public record RegisterDto(string EmailAddress, string Password, string ConfirmPassword, string FirstName, string LastName, string PhoneNumber);
    
}
