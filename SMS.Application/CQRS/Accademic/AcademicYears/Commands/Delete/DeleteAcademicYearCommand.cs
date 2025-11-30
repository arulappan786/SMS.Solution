using MediatR;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Commands.Delete
{
    public class DeleteAcademicYearCommand : IRequest<ServiceResponse>
    {
        public required Guid Id { get; set; }
    }
}
