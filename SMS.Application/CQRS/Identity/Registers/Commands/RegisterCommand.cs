using MediatR;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Identity.Registers.Commands
{
    public record RegisterCommand(
        string UserName,
        string Email,
        string Password,
        string ConfirmPassword) : IRequest<ServiceResponse>;
}
