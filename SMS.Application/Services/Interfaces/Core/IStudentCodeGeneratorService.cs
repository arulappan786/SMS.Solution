namespace SMS.Application.Services.Interfaces.Core
{
    public interface IStudentCodeGeneratorService
    {
        Task<string> GenerateNewStudentCodeAsync(DateTime enrollmentDate);
    }
}
