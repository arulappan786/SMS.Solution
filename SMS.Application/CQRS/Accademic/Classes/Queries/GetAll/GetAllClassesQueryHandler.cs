using AutoMapper;
using MediatR;
using SMS.Application.DTOs.Accademic.Classes;
using SMS.Application.DTOs.Common;
using SMS.Domain.Interfaces.Repositories.Academic;

namespace SMS.Application.CQRS.Accademic.Classes.Queries.GetAll
{

    public class GetAllClassesQueryHandler(IClassesRepository repository, IMapper mapper)
        : IRequestHandler<GetAllClasssesQuery, PaginatedResultDto<ClassesDto>>
    {
        public async Task<PaginatedResultDto<ClassesDto>> Handle(GetAllClasssesQuery request, CancellationToken cancellationToken)
        {
            // 1. Call Repository to get paged data and total count
            var (classess, totalCount) = await repository.GetAllPaginatedAsync(
                pageNumber: request.PageNumber, pageSize: request.PageSize, orderByExpression: a => a.Id, ascending: true, cancellationToken: cancellationToken);

            // 2. Map the entities to DTOs
            var classessDto = mapper.Map<IEnumerable<ClassesDto>>(classess);

            // 3. Construct the final PaginatedResultDto
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PaginatedResultDto<ClassesDto>
            {
                Items = classessDto,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}
