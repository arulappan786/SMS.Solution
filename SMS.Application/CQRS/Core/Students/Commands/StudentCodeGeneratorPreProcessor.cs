using MediatR.Pipeline;
using SMS.Application.Services.Interfaces.Core;

namespace SMS.Application.CQRS.Core.Students.Commands
{
    // (Simplified for illustration)
    public class StudentCodeGeneratorPreProcessor(IStudentCodeGeneratorService studentCodeGeneratorService) : IRequestPreProcessor<CreateStudentCommand>
    {
        public async Task Process(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            // 1. Generate the unique code
            string generatedCode = await studentCodeGeneratorService.GenerateNewStudentCodeAsync(DateTime.UtcNow);

            // 2. Inject the code using the interface setter
            ((IStudentHasInternalIds)request).StudentCode = generatedCode;
            
        }
    }
}
