using SMS.Domain.Enums;
using SMS.Domain.ValueObjects;

namespace SMS.Application.DTOs.Core
{
    public record UpdateStudentDto(Guid StudentId,
                                   Guid UserId,
                                   Guid CurrentClassId,
                                   FullName FullName,
                                   Address HomeAddress,
                                   DateTime DateOfBirth,
                                   Gender Gender,
                                   string Email,
                                   string StudentCode,
                                   DateTime EnrollmentDate);
}
