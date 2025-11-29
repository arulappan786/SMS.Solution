using MediatR;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Core.Students.Commands.Delete
{
    public class DeleteStudentCommand : IRequest<ServiceResponse>
    {
        // The only required data is the ID of the student to delete
        public required Guid Id { get; set; }
    }
}
