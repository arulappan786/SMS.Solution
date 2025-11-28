using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Domain.Enums;
using SMS.Domain.ValueObjects;

namespace SMS.Application.CQRS.Core.Students.Commands.CreateStudent
{
    public record CreateStudentCommand : IRequest<ServiceResponse>, IStudentHasInternalIds
    {
        // 1. Internal Fields: Explicitly implemented to hide them from the API JSON contract
        Guid? IStudentHasInternalIds.UserId { get; set; }
        string? IStudentHasInternalIds.StudentCode { get; set; }

        // 2. Public Fields (Required from API Client)
        public required FullName FullName { get; init; }
        public required Address HomeAddress { get; init; }
        public required DateTime DateOfBirth { get; init; }
        public required Gender Gender { get; init; }
        public required string Email { get; init; }

        // Optional: Public read-only properties for safe access in the Handler
        public Guid? UserId => ((IStudentHasInternalIds)this).UserId;
        public string? StudentCode => ((IStudentHasInternalIds)this).StudentCode;
    }
}