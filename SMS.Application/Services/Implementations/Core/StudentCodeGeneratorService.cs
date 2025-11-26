using Microsoft.Extensions.Options;
using SMS.Application.Services.Interfaces.Core;
using SMS.Domain.Interfaces.Repositories;

namespace SMS.Application.Services.Implementations.Core
{
    public class StudentCodeGeneratorService(IOptions<StudentSettings> options,
                                             IStudentRepository _repository) : IStudentCodeGeneratorService
    {
        public async Task<string> GenerateNewStudentCodeAsync(DateTime enrollmentDate)
        {
            string year = enrollmentDate.Year.ToString();
            int nextId = (await _repository.GetTotalStudentCountAsync() + 1);
            string sequentialPart = nextId.ToString($"D{options.Value.CodeLength}");
            string newStudentCode = $"{options.Value.CodePrefix}{year}{sequentialPart}";
            return newStudentCode;
        }
    }
}