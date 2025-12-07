using AutoMapper;
using MediatR;
using SMS.Application.DTOs.Accademic.AcademicYears;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Service;
using SMS.Domain.Interfaces.Repositories.Academic;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Queries.GetAll
{
    // Handler signature remains focused on the data structure
    public class GetAllAcademicYearsQueryHandler(IAcademicYearRepository repository, IMapper mapper) 
        : IRequestHandler<GetAllAcademicYearsQuery, ServiceResponse<PaginatedResultDto<AcademicYearDto>>>
    {
        public async Task<ServiceResponse<PaginatedResultDto<AcademicYearDto>>> Handle(GetAllAcademicYearsQuery request, CancellationToken cancellationToken)
        {
            // 1. Call Repository to get paged data and total count
            var (academicYears, totalCount) = await repository.GetAllPaginatedAsync(
                pageNumber: request.PageNumber, pageSize: request.PageSize, orderByExpression: a => a.Id, ascending: true, cancellationToken: cancellationToken);

            // 2. Map the entities to DTOs
            var academicYearsDtos = mapper.Map<List<AcademicYearDto>>(academicYears);

            // 3. Construct the final PaginatedResultDto
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            // Direct return of the data structure is clean for successful queries
            return ServiceResponse<PaginatedResultDto<AcademicYearDto>>.Success(data: new PaginatedResultDto<AcademicYearDto>
            {
                Items = academicYearsDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            });
        }
    }
}