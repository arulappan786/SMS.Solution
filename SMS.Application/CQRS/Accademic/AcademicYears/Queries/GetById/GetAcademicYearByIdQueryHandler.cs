using AutoMapper;
using MediatR;
using SMS.Application.DTOs.Accademic.AcademicYears;
using SMS.Application.DTOs.Service;
using SMS.Application.Exceptions;
using SMS.Domain.Interfaces.Repositories.Academic;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Queries.GetById
{
    // The handler now returns the concrete DTO type
    public class GetAcademicYearByIdQueryHandler(IAcademicYearRepository repository, IMapper mapper)
        : IRequestHandler<GetAcademicYearByIdQuery, ServiceResponse<AcademicYearDto>>
    {
        public async Task<ServiceResponse<AcademicYearDto>> Handle(GetAcademicYearByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Retrieve the entity
            var academicYear = await repository.GetAsync(request.Id, cancellationToken);

            // 2. Handle Not Found by throwing an exception
            if (academicYear == null)
            {
                // Throw a custom application-level exception.
                // You must define EntityNotFoundException in your application.
                throw new EntityNotFoundException($"Academic Year with ID {request.Id} not found.");
            }

            // 3. Map and return the concrete DTO
            var mapped = mapper.Map<AcademicYearDto>(academicYear);

            return ServiceResponse<AcademicYearDto>.Success(data: mapped);
        }
    }
}