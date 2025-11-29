using MediatR;
using SMS.Application.DTOs.Accademic.AcademicYears;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Queries.GetById
{
    public record GetAcademicYearByIdQuery : IRequest<AcademicYearDto>
    {
        public required Guid Id { get; init; }
    }
}
