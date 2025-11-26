using SMS.Application.Services.Interfaces.Core;
using SMS.Domain.Interfaces.Repositories;

namespace SMS.Application.Services.Implementations.Core
{
    public class StudentCodeGeneratorService(StudentCodeSettings _settings,
                                             IStudentRepository _repository) : IStudentCodeGeneratorService
    {
        public async Task<string> GenerateNewStudentCodeAsync(DateTime enrollmentDate)
        {
            string year = enrollmentDate.Year.ToString();
            int nextId = (await _repository.GetTotalStudentCountAsync() + 1);
            string sequentialPart = nextId.ToString($"D{_settings.CodeLength}");
            return $"{_settings.CodePrefix}-{year}-{sequentialPart}";
        }
    }
}