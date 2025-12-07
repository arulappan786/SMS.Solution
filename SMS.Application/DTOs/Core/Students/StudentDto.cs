using SMS.Domain.ValueObjects;

namespace SMS.Application.DTOs.Core.Students
{
    public record StudentDto(Guid Id,
                             Guid UserId,
                             Guid CurrentClassId,
                             string CurrentClassName,
                             FullName FullName,
                             Address HomeAddress,
                             DateOnly DateOfBirth,
                             string Gender,
                             string Email,
                             string StudentCode,
                             DateTime EnrollmentDate);
    
}