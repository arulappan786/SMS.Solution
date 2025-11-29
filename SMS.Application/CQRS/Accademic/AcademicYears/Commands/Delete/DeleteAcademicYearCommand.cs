using MediatR;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Commands.Delete
{
    public class DeleteAcademicYearCommand : IRequest<ServiceResponse>
    {
        // The only required data is the ID of the student to delete
        public required Guid Id { get; set; }
    }
}
