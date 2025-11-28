using MediatR;
using SMS.Application.DTOs.Core;
using SMS.Application.Services.Interfaces.Core;

namespace SMS.Application.CQRS.Core.Students.Queries.GetStudentById
{
    public class GetStudentByIdQueryHandler(IStudentService studentService) : IRequestHandler<GetStudentByIdQuery, StudentDto>
    {
        public Task<StudentDto> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            return studentService.GetStudentByIdAsync(request.StudentId, cancellationToken);
        }
    }
}
