using SMS.Application.CQRS.Core.Students.Commands.CreateStudent;
using SMS.Application.CQRS.Core.Students.Queries.GetStudentList;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Core;
using SMS.Application.DTOs.Service;

namespace SMS.Application.Services.Interfaces.Core
{
    public interface IStudentService
    {
        Task<PaginatedResultDto<StudentDto>> GetAllStudentAsync(GetAllStudentsQuery request, CancellationToken cancellationToken);

        Task<StudentDto> GetStudentByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<ServiceResponse> OnboardNewStudentAsync(CreateStudentCommand student, CancellationToken cancellationToken);

    }
}