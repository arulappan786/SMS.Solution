using MediatR;
using Microsoft.Extensions.Logging;
using SMS.Application.DTOs.Service;
using SMS.Domain.Interfaces.Repositories.Common;
using SMS.Domain.Interfaces.Repositories.Core;

namespace SMS.Application.CQRS.Core.Students.Commands.Delete
{
    public class DeleteStudentCommandHandler(IStudentRepository repository,
            IUnitOfWork unitOfWork,
            ILogger<DeleteStudentCommandHandler> logger) : IRequestHandler<DeleteStudentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting student deletion for ID: {StudentId}", request.Id);

            // 1. Retrieve the existing entity by ID
            var studentToDelete = await repository.GetAsync(request.Id, cancellationToken);

            if (studentToDelete == null)
            {
                logger.LogWarning("Delete failed: Student ID {StudentId} not found.", request.Id);
                return new ServiceResponse { Succeeded = false, Message = $"Student with ID {request.Id} not found." };
            }

            // 2. Remove the entity
             await repository.DeleteAsync(request.Id, cancellationToken);

            // 3. Commit transaction
            await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInformation("Successfully deleted student: {StudentId}", request.Id);

            return new ServiceResponse { Succeeded = false, Message = $"Student with ID {request.Id} was successfully deleted." };
        }
    }
}
