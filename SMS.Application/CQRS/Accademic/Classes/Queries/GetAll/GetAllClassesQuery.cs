using MediatR;
using SMS.Application.DTOs.Accademic.Classes;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Accademic.Classes.Queries.GetAll
{
    public class GetAllClassesQuery 
        : PaginationQuery, IRequest<ServiceResponse<PaginatedResultDto<ClassesDto>>>
    {

    }
}
