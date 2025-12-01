using MediatR;
using SMS.Application.DTOs.Accademic.AcademicYears;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Queries.GetAll
{
    public class GetAllAcademicYearsQuery 
        : PaginationQuery, IRequest<ServiceResponse<PaginatedResultDto<AcademicYearDto>>> 
    { 
    
    }
}
