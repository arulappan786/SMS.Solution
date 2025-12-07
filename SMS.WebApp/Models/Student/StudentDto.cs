namespace SMS.WebApp.Models.Student
{
    public record StudentDto(Guid StudentId,
                             Guid UserId,
                             Guid CurrentClassId,
                             FullName FullName,
                             Address HomeAddress,
                             DateOnly DateOfBirth,
                             Gender Gender,
                             string Email,
                             string StudentCode,
                             DateTime EnrollmentDate);
}
