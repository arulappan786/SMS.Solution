using MediatR;
using SMS.Application.DTOs.Accademic.Classes;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Accademic.Classes.Queries.GetById
{
    public class GetClassesByIdQuery : IRequest<ServiceResponse<ClassesDto>>
    {
        public Guid Id { get; set; }
    }
}
