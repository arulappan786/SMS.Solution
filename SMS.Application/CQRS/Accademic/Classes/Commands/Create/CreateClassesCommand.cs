using MediatR;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Accademic.Classes.Commands.Create
{
    public class CreateClassesCommand : IRequest<ServiceResponse>
    {
        public required Guid AcademicYearId { get; set; }

        public required string Name { get; set; }
        
        public required int MaxCapacity { get; set; }
    }
}
