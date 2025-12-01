using MediatR;
using SMS.Application.DTOs.Core.Students;
using SMS.Application.DTOs.Service;

namespace SMS.Application.CQRS.Core.Students.Queries.GetById
{
    public record GetStudentByIdQuery : IRequest<ServiceResponse<StudentDto>>
    {
        public required Guid Id { get; init; }
    }
}
