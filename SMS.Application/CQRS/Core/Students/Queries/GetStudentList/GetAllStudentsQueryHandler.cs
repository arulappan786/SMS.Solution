using MediatR;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Core;
using SMS.Application.Services.Interfaces.Core;

namespace SMS.Application.CQRS.Core.Students.Queries.GetStudentList
{

    public class GetAllStudentsQueryHandler(IStudentService studentService) : IRequestHandler<GetAllStudentsQuery, PaginatedResultDto<StudentDto>>
    {
        public async Task<PaginatedResultDto<StudentDto>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {
            return await studentService.GetAllStudentAsync(request, cancellationToken);
        }
    }
}
