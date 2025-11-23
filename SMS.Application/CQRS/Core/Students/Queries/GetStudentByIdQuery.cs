using MediatR;
using SMS.Application.DTOs.Core;

namespace SMS.Application.CQRS.Core.Students.Queries
{
    public record GetStudentByIdQuery : IRequest<StudentDto>
    {
        public required int StudentId { get; init; }
    }
}
