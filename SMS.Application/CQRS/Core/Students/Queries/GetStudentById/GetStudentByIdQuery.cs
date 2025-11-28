using MediatR;
using SMS.Application.DTOs.Core;

namespace SMS.Application.CQRS.Core.Students.Queries.GetStudentById
{
    public record GetStudentByIdQuery : IRequest<StudentDto>
    {
        public required Guid StudentId { get; init; }
    }
}
