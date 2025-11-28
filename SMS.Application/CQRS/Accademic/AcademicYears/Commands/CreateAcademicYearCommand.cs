using MediatR;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Commands
{
    public class CreateAcademicYearCommand : IRequest<ServiceResponse>
    {
        public required string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrent { get; set; }
    }
}
