using MediatR;
using SMS.Application.DTOs.Accademic.Classes;

namespace SMS.Application.CQRS.Accademic.Classes.Queries.GetById
{
    public class GetClassesByIdQuery : IRequest<ClassesDto>
    {
        public Guid Id { get; set; }
    }
}
