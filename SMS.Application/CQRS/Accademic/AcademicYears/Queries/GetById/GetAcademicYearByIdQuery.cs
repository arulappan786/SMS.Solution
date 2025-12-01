using MediatR;
using SMS.Application.DTOs.Accademic.AcademicYears;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Queries.GetById
{
    public record GetAcademicYearByIdQuery : IRequest<ServiceResponse<AcademicYearDto>>
    {
        public required Guid Id { get; init; }
    }
}
