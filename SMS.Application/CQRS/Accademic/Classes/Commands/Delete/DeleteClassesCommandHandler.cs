using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Application.CQRS.Accademic.Classes.Commands.Delete
{
    public class DeleteClassesCommandHandler(
        IClassesRepository repository,
        IUnitOfWork unitOfWork,
        IAppLogger<DeleteClassesCommandHandler> logger)
        : IRequestHandler<DeleteClassesCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteClassesCommand request, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Starting Class deletion for ID: {request.Id}");

            try
            {
                // 1. Retrieve the existing entity
                // Note: Renamed variable for clarity
                var classToDelete = await repository.GetAsync(request.Id, cancellationToken);

                if (classToDelete == null)
                {
                    logger.LogWarning($"Class deletion failed: ID {request.Id} not found.");

                    // Use ServiceResponse.Failure
                    return ServiceResponse.Failure($"Class with ID {request.Id} not found.");
                }

                // 2. Remove the entity
                // Note: Using the entity's ID for deletion, though passing the tracked entity is often safer.
                await repository.DeleteAsync(request.Id, cancellationToken);

                // 3. Commit transaction
                int result = await unitOfWork.CommitAsync(cancellationToken);

                if (result == 0)
                {
                    logger.LogWarning($"Class deletion failed: ID {request.Id} found but no records were affected during commit.");
                    // Use ServiceResponse.Failure
                    return ServiceResponse.Failure($"Class deletion failed: No records were affected for ID {request.Id}.");
                }

                // 4. Success
                logger.LogInfo($"Successfully deleted Class with ID: {request.Id}");

                // Use ServiceResponse.Success
                return ServiceResponse.Success($"Class with ID {request.Id} was successfully deleted.");
            }
            catch (Exception ex)
            {
                // Handle unhandled exceptions during database or repository operations
                logger.LogError(ex, $"Critical error during Class deletion for ID: {request.Id}");

                // Use ServiceResponse.Failure for unhandled exceptions
                return ServiceResponse.Failure($"An unexpected error occurred during deletion of Class ID {request.Id}. Error: {ex.Message}");
            }
        }
    }
}