using MediatR;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Accademic.Classes.Commands.Update
{
    public class UpdateClassesCommand : IRequest<ServiceResponse>
    {
        public required Guid Id { get; set; }

        public Guid AcademicYearId { get; set; }

        public string? Name { get; set; }

        public int? MaxCapacity { get; set; }
    }
}
