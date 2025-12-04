using MediatR;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Identity.Logins.Commands
{
    public class LoginCommand : IRequest<ServiceResponse>
    {
        public required string EmailAddess { get; set; }
        public required string Password { get; set; }
    }
}
