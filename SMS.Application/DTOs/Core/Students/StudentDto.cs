using SMS.Domain.Enums;
using SMS.Domain.ValueObjects;

namespace SMS.Application.DTOs.Core.Students
{
    public record StudentDto(Guid StudentId,
                             Guid UserId,
                             Guid CurrentClassId,
                             FullName FullName,
                             Address HomeAddress,
                             DateTime DateOfBirth,
                             string Gender,
                             string Email,
                             string StudentCode,
                             DateTime EnrollmentDate);
    
}