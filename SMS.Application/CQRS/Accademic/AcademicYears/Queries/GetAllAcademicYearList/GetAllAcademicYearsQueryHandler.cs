using AutoMapper;
using MediatR;
using SMS.Application.DTOs.Accademic.AcademicYears;
using SMS.Application.DTOs.Common;
using SMS.Domain.Interfaces.Repositories.Academic;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Queries.GetAllAcademicYearList
{

    public class GetAllAcademicYearsQueryHandler(IAcademicYearRepository repository, IMapper mapper) : IRequestHandler<GetAllAcademicYearsQuery, PaginatedResultDto<AcademicYearDto>>
    {
        public async Task<PaginatedResultDto<AcademicYearDto>> Handle(GetAllAcademicYearsQuery request, CancellationToken cancellationToken)
        {
            // 1. Call Repository to get paged data and total count
            var (academicYears, totalCount) = await repository.GetAllPaginatedAsync(
                pageNumber: request.PageNumber, pageSize: request.PageSize, orderByExpression: a => a.Id, ascending: true, cancellationToken: cancellationToken);

            // 2. Map the entities to DTOs
            var accademicyearsDtos = mapper.Map<IEnumerable<AcademicYearDto>>(academicYears);

            // 3. Construct the final PaginatedResultDto
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PaginatedResultDto<AcademicYearDto>
            {
                Items = accademicyearsDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }        
    }
}
