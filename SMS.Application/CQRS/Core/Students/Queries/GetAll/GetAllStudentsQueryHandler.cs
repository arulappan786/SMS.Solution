using AutoMapper;
using MediatR;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Core.Students;
using SMS.Application.DTOs.Service;
using SMS.Domain.Interfaces.Repositories.Core;

namespace SMS.Application.CQRS.Core.Students.Queries.GetAll
{
    // Handler returns the concrete PaginatedResultDto<StudentDto>
    public class GetAllStudentsQueryHandler(IStudentRepository studentRepository, IMapper mapper)
        : IRequestHandler<GetAllStudentsQuery, ServiceResponse<PaginatedResultDto<StudentDto>>>
    {
        public async Task<ServiceResponse<PaginatedResultDto<StudentDto>>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
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

            return ServiceResponse<PaginatedResultDto<StudentDto>>
                .Success(data: new PaginatedResultDto<StudentDto>
            {
                Items = studentDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            });
        }
    }
}