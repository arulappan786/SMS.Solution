using MediatR;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Core;

namespace SMS.Application.CQRS.Core.Students.Queries.GetStudentList
{
    public class GetAllStudentsQuery : PaginationQuery, IRequest<PaginatedResultDto<StudentDto>> 
    { 
    
    }
}
