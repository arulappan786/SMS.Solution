using SMS.Application.DTOs.Identity;

namespace SMS.Application.Services.Identity
{
    public interface IIdentityService
    {
        Task<(bool Success, string Message, LoggedInUserDto? UserDto)> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken);
    }
}
