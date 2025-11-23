using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Interfaces.Core;

namespace SMS.Application.CQRS.Core.Students.Commands
{
    public class CreateStudentCommandHandler(IStudentService studentService): IRequestHandler<CreateStudentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            var result = await studentService.CreateStudentAsync(request);
            return result;
        }
    }
}