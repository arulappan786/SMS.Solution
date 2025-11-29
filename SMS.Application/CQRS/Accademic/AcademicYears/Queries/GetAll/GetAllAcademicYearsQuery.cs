using MediatR;
using SMS.Application.DTOs.Accademic.AcademicYears;
using SMS.Application.DTOs.Common;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Queries.GetAll
{
    public class GetAllAcademicYearsQuery : PaginationQuery, IRequest<PaginatedResultDto<AcademicYearDto>> 
    { 
    
    }
}
