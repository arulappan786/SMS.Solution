using AutoMapper;
using MediatR;
using SMS.Application.DTOs.Accademic.Classes;
using SMS.Application.Exceptions;
using SMS.Domain.Interfaces.Repositories.Academic;

namespace SMS.Application.CQRS.Accademic.Classes.Queries.GetById
{
    public class GetClassesByIdQueryHandler(IClassesRepository repository, IMapper mapper)
        : IRequestHandler<GetClassesByIdQuery, ClassesDto>
    {
        public async Task<ClassesDto> Handle(GetClassesByIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Retrieve the entity (Class)
            // Note: Renamed variable from 'accademicyear' to 'classEntity' for domain accuracy.
            var classEntity = await repository.GetAsync(request.Id, cancellationToken);

            // 2. Handle Not Found by throwing an exception
            if (classEntity == null)
            {
                // Throw the custom application-level exception
                throw new EntityNotFoundException(nameof(Domain.Entities.Academic.Classes), request.Id);
            }

            // 3. Map and return the concrete DTO
            var mapped = mapper.Map<ClassesDto>(classEntity);

            return mapped;
        }
    }
}