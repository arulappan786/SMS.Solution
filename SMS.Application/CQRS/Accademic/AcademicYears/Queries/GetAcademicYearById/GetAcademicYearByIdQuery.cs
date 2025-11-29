using MediatR;
using SMS.Application.DTOs.Accademic.AcademicYears;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Queries.GetAcademicYearById
{
    public record GetAcademicYearByIdQuery : IRequest<AcademicYearDto>
    {
        public required Guid Id { get; init; }
    }
}
