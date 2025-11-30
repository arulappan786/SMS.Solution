using MediatR;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Accademic.Classes.Commands.Delete
{
    public class DeleteClassesCommand : IRequest<ServiceResponse>
    {
        public Guid Id { get; set; }
    }
}
