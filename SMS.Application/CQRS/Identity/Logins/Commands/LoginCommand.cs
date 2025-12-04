using MediatR;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Identity.Logins.Commands
{
    public class LoginCommand : IRequest<ServiceResponse>
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
