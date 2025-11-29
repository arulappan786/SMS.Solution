using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Domain.Enums;
using SMS.Domain.ValueObjects;

namespace SMS.Application.CQRS.Core.Students.Commands.UpdateStudent
{
    public class UpdateStudentCommand : IRequest<ServiceResponse>
    {
        public required Guid Id { get; set; }
        public required FullName FullName { get; set; }
        public required Address HomeAddress { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public required string Email { get; set; }
    }
}
