using MediatR;
using Microsoft.EntityFrameworkCore;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Application.CQRS.Accademic.Classes.Commands.Update
{
    // Note: Corrected the namespace from Classess.Commands.Update to Classes.Commands.Update
    public class UpdateClassesCommandHandler(
        IClassesRepository repository,
        IUnitOfWork unitOfWork,
        IAppLogger<UpdateClassesCommandHandler> logger)
        : IRequestHandler<UpdateClassesCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateClassesCommand request, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Starting Classes update for ID: {request.Id}");

            try
            {
                // 1. Retrieve the existing entity
                var classesToUpdate = await repository.GetAsync(request.Id, cancellationToken);

                if (classesToUpdate == null)
                {
                    logger.LogWarning($"Classes update failed: ID {request.Id} not found.");
                    // Use ServiceResponse.Failure for Not Found
                    return ServiceResponse.Failure($"Classes with ID {request.Id} not found.");
                }

                // 2. Apply updates from the command to the entity
                // The null-coalescing operator (??) ensures properties are updated only if provided in the request (i.e., not null).
                classesToUpdate.Name = request.Name ?? classesToUpdate.Name;
                classesToUpdate.AcademicYearId = request.AcademicYearId ?? classesToUpdate.AcademicYearId;
                classesToUpdate.MaxCapacity = request.MaxCapacity ?? classesToUpdate.MaxCapacity;

                // 3. Update the repository (often unnecessary if tracked, but kept for explicit marking)
                // Note: Assuming UpdateAsync either takes the tracked entity or the ID and entity.
                await repository.UpdateAsync(classesToUpdate.Id, classesToUpdate, cancellationToken);

                // 4. Commit transaction
                await unitOfWork.CommitAsync(cancellationToken);

                logger.LogInfo($"Successfully updated Classes: {request.Id}");

                // Use ServiceResponse.Success
                return ServiceResponse.Success(
                    $"Class '{classesToUpdate.Name}' was successfully updated.",
                    new { classesToUpdate.Id, classesToUpdate.Name } // Return updated ID/Name
                );
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency conflicts (e.g., another user updated the record simultaneously)
                logger.LogError(ex, $"Concurrency error during update of Classes ID: {request.Id}");

                // Use ServiceResponse.Failure
                return ServiceResponse.Failure("The record you are trying to update has been modified by another user. Please refresh and try again.");
            }
            catch (Exception ex)
            {
                // Log all other unexpected errors
                logger.LogError(ex, $"Error occurred while updating Classes ID: {request.Id}");

                // Use ServiceResponse.Failure
                return ServiceResponse.Failure("An unexpected error occurred during the Class update process. Please contact support.");
            }
        }
    }
}