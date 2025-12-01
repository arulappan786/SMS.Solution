using MediatR;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Core.Students;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Core.Students.Queries.GetAll
{
    public class GetAllStudentsQuery 
        : PaginationQuery, IRequest<ServiceResponse<PaginatedResultDto<StudentDto>>> 
    { 
    
    }
}
