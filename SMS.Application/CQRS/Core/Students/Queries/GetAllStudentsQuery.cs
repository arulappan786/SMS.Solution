using MediatR;
using SMS.Application.DTOs.Core;

namespace SMS.Application.CQRS.Core.Students.Queries
{
    public record GetAllStudentsQuery : IRequest<List<StudentDto>>
    {
        public string? SearchTerm { get; init; }
    }
}
