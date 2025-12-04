using MediatR;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Identity.Logins.Commands
{
    // The command carries the UserId of the user whose tokens are to be revoked.
    public record AdminRevokeTokensCommand(Guid TargetUserId, Guid AdminUserId) : IRequest<ServiceResponse>;
}
