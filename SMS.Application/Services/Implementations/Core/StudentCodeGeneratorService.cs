using SMS.Application.Services.Interfaces.Core;
using SMS.Domain.Interfaces.Repositories;

namespace SMS.Application.Services.Implementations.Core
{
    public record StudentCodeSettings
    {
        public required string CodePrefix { get; init; }
        public required int CodeLength { get; init; }

    }

    public class StudentCodeGeneratorService(StudentCodeSettings _settings,
                                             IStudentRepository _repository) : IStudentCodeGeneratorService
    {
        public async Task<string> GenerateNewStudentCodeAsync(DateTime enrollmentDate)
        {
            string year = enrollmentDate.Year.ToString().Substring(2);

            int nextId = (await _repository.GetTotalStudentCountAsync() + 1);

            string sequentialPart = nextId.ToString($"D{_settings.CodeLength}");

            return $"{_settings.CodePrefix}-{year}-{sequentialPart}";
        }
    }
}