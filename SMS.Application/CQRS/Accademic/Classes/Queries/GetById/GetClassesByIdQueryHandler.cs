using AutoMapper;
using MediatR;
using SMS.Application.DTOs.Accademic.Classes;
using SMS.Domain.Interfaces.Repositories.Academic;

namespace SMS.Application.CQRS.Accademic.Classes.Queries.GetById
{
    public class GetClassesByIdQueryHandler(IClassesRepository repository, IMapper mapper) : IRequestHandler<GetClassesByIdQuery, ClassesDto>
    {
        public async Task<ClassesDto> Handle(GetClassesByIdQuery request, CancellationToken cancellationToken)
        {
            var accademicyear = await repository.GetAsync(request.Id, cancellationToken);
            var mapped = mapper.Map<ClassesDto>(accademicyear);
            return mapped;
        }
    }
}
