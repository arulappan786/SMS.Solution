using AutoMapper;
using MediatR;
using SMS.Application.DTOs.Accademic.AcademicYears;
using SMS.Domain.Interfaces.Repositories.Academic;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Queries.GetAcademicYearById
{
    public class GetAcademicYearByIdQueryHandler(IAcademicYearRepository repository, IMapper mapper) : IRequestHandler<GetAcademicYearByIdQuery, AcademicYearDto>
    {
        public async Task<AcademicYearDto> Handle(GetAcademicYearByIdQuery request, CancellationToken cancellationToken)
        {
            var accademicyear = await repository.GetAsync(request.Id, cancellationToken);
            var mapped = mapper.Map<AcademicYearDto>(accademicyear);
            return mapped;
        }
    }
}
