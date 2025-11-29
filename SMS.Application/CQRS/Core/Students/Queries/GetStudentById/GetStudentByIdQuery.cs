using MediatR;
using SMS.Application.DTOs.Core.Students;

namespace SMS.Application.CQRS.Core.Students.Queries.GetStudentById
{
    public record GetStudentByIdQuery : IRequest<StudentDto>
    {
        public required Guid Id { get; init; }
    }
}
