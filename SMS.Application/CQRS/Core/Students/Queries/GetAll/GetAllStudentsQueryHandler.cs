using AutoMapper;
using MediatR;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Core.Students;
using SMS.Domain.Interfaces.Repositories.Core;

namespace SMS.Application.CQRS.Core.Students.Queries.GetAll
{
    // Handler returns the concrete PaginatedResultDto<StudentDto>
    public class GetAllStudentsQueryHandler(IStudentRepository studentRepository, IMapper mapper)
        : IRequestHandler<GetAllStudentsQuery, PaginatedResultDto<StudentDto>>
    {
        public async Task<PaginatedResultDto<StudentDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {
            // 1. Call Repository to get paged data and total count
            var (students, totalCount) = await studentRepository.GetAllPaginatedAsync(
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                orderByExpression: s => s.StudentCode, // Ordering by StudentCode
                ascending: true, // Assuming ascending order is default/desired
                cancellationToken: cancellationToken);

            // 2. Map the entities to DTOs
            var studentDtos = mapper.Map<IEnumerable<StudentDto>>(students);

            // 3. Construct the final PaginatedResultDto
            // Calculate total pages safely
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            // Direct return of the structured data is the best practice for successful queries.
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