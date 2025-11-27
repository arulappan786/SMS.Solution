using SMS.Application.CQRS.Core.Students.Commands;
using SMS.Application.DTOs.Service;

namespace SMS.Application.Services.Interfaces.Core
{
    public interface IStudentService
    {
        Task<ServiceResponse> OnboardNewStudentAsync(CreateStudentCommand student, CancellationToken cancellationToken);
    }
}