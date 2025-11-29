using MediatR;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Commands.Create
{
    public class CreateAcademicYearCommand : IRequest<ServiceResponse>
    {
        public required string Name { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool IsCurrent => StartDate <= DateOnly.FromDateTime(DateTime.Today) &&
                          EndDate >= DateOnly.FromDateTime(DateTime.Today);
    }
}
