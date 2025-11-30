using MediatR;
using SMS.Application.DTOs.Accademic.Classes;
using SMS.Application.DTOs.Common;

namespace SMS.Application.CQRS.Accademic.Classes.Queries.GetAll
{
    public class GetAllClassesQuery : PaginationQuery, IRequest<PaginatedResultDto<ClassesDto>>
    {

    }
}
