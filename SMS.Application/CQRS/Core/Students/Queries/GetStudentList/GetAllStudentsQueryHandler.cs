using AutoMapper;
using MediatR;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Core.Students;
using SMS.Domain.Interfaces.Repositories.Core;

namespace SMS.Application.CQRS.Core.Students.Queries.GetStudentList
{

    public class GetAllStudentsQueryHandler(IStudentRepository studentRepository, IMapper mapper) : IRequestHandler<GetAllStudentsQuery, PaginatedResultDto<StudentDto>>
    {
        public async Task<PaginatedResultDto<StudentDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {
            // 1. Call Repository to get paged data and total count
            var (students, totalCount) = await studentRepository.GetAllPaginatedAsync(
                request.PageNumber, request.PageSize, cancellationToken);

            // 2. Map the entities to DTOs
            var studentDtos = mapper.Map<IEnumerable<StudentDto>>(students);

            // 3. Construct the final PaginatedResultDto
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PaginatedResultDto<StudentDto>
            {
                Items = studentDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}
