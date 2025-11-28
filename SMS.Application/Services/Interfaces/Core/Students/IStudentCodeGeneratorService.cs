namespace SMS.Application.Services.Interfaces.Core.Students
{
    public interface IStudentCodeGeneratorService
    {
        Task<string> GenerateNewStudentCodeAsync(DateTime enrollmentDate);
    }
}
