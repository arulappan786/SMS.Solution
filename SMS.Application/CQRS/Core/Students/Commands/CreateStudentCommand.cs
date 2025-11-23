using MediatR;
using SMS.Domain.Enums;
using SMS.Domain.ValueObjects;

namespace SMS.Application.CQRS.Core.Students.Commands
{
    public record CreateStudentCommand : IRequest<int>
    {
        public required string UserId { get; init; }
        public required FullName FullName { get; init; }
        public required Address HomeAddress { get; init; }
        public required DateTime DateOfBirth { get; init; }
        public required Gender Gender { get; init; }
        public required string Email { get; init; }
        public required string StudentCode { get; init; }
    }
}
