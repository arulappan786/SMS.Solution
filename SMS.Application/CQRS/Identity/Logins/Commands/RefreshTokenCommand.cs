using MediatR;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Identity.Logins.Commands
{
    // The command carries the request data and expects a ServiceResponse containing the new tokens.
    public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<ServiceResponse>;
}