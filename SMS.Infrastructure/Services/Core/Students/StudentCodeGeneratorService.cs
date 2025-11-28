using Microsoft.Extensions.Options;
using SMS.Application.Configs;
using SMS.Application.Services.Core.Students;
using SMS.Domain.Interfaces.Repositories;

namespace SMS.Infrastructure.Services.Core.Students
{
    public class StudentCodeGeneratorService(IOptions<StudentSettings> studentOptions,
                                             IStudentRepository _repository) : IStudentCodeGeneratorService
    {
        private readonly StudentSettings _settings = studentOptions.Value;

        public async Task<string> GenerateNewStudentCodeAsync(DateTime enrollmentDate)
        {
            string year = enrollmentDate.Year.ToString();
            int nextId = await _repository.GetTotalStudentCountAsync() + 1;
            string sequentialPart = nextId.ToString($"D{_settings.CodeLength}");
            string newStudentCode = $"{_settings.CodePrefix}{year}{sequentialPart}";
            return newStudentCode;
        }
    }
}