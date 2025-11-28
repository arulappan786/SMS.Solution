namespace SMS.Application.Services.Core.Students
{
    public interface IStudentCodeGeneratorService
    {
        Task<string> GenerateNewStudentCodeAsync(DateTime enrollmentDate);
    }
}
