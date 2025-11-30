using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Domain.Interfaces.Repositories.Common;
using SMS.Domain.Interfaces.Repositories.Core;

namespace SMS.Application.CQRS.Core.Students.Commands.Delete
{
    public class DeleteStudentCommandHandler(IStudentRepository repository,
                                             IUnitOfWork unitOfWork,
                                             IAppLogger<DeleteStudentCommandHandler> logger)
        : IRequestHandler<DeleteStudentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Starting student deletion for ID: {request.Id}");

            try
            {
                // 1. Retrieve the existing entity by ID
                var studentToDelete = await repository.GetAsync(request.Id, cancellationToken);

                if (studentToDelete == null)
                {
                    logger.LogWarning($"Delete failed: Student ID {request.Id} not found.");

                    // Use ServiceResponse.Failure for Not Found
                    return ServiceResponse.Failure($"Student with ID {request.Id} not found.");
                }

                // 2. Remove the entity
                await repository.DeleteAsync(studentToDelete.Id, cancellationToken); // Passing entity or ID

                // 3. Commit transaction
                int result = await unitOfWork.CommitAsync(cancellationToken);

                if (result == 0)
                {
                    logger.LogWarning($"Delete failed: Student ID {request.Id} found but no records were affected during commit.");
                    return ServiceResponse.Failure($"Student deletion failed: No records were affected for ID {request.Id}.");
                }

                // 4. Success
                logger.LogInfo($"Successfully deleted student: {request.Id}");

                // Use ServiceResponse.Success (Corrected original logic which returned Succeeded = false)
                return ServiceResponse.Success($"Student with ID {request.Id} was successfully deleted.");
            }
            catch (Exception ex)
            {
                // Handle unhandled exceptions during database or repository operations
                logger.LogError(ex, $"Critical error during student deletion for ID: {request.Id}");

                // Use ServiceResponse.Failure for unhandled exceptions
                return ServiceResponse.Failure($"An unexpected error occurred during deletion of Student ID {request.Id}. Error: {ex.Message}");
            }
        }
    }
}