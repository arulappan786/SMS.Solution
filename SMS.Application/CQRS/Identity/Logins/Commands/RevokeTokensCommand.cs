using MediatR;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Identity.Logins.Commands
{
    // The command carries the User ID extracted securely from the JWT claim.
    public record RevokeTokensCommand(Guid UserId) : IRequest<ServiceResponse>;
}
