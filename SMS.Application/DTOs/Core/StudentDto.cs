using SMS.Domain.Enums;
using SMS.Domain.ValueObjects;

namespace SMS.Application.DTOs.Core
{
    public record StudentDto(int StudentId,
                             int UserId,
                             int CurrentClassId,
                             FullName FullName,
                             Address HomeAddress,
                             DateTime DateOfBirth,
                             Gender Gender,
                             string Email,
                             string StudentCode,
                             DateTime EnrollmentDate);
    
}