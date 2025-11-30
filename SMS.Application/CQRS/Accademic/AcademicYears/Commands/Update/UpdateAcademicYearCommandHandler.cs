using MediatR;
using Microsoft.EntityFrameworkCore;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Commands.Update
{
    // Note: Corrected the handler name to match the command and domain
    public class UpdateAcademicYearCommandHandler(
        IAcademicYearRepository repository,
        IUnitOfWork unitOfWork,
        IAppLogger<UpdateAcademicYearCommandHandler> logger)
        : IRequestHandler<UpdateAcademicYearCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateAcademicYearCommand request, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Starting AcademicYear update for ID: {request.Id}");

            try
            {
                // 1. Retrieve the existing entity
                var academicYearToUpdate = await repository.GetAsync(request.Id, cancellationToken);

                if (academicYearToUpdate == null)
                {
                    // Use ServiceResponse.Failure for Not Found
                    logger.LogWarning($"AcademicYear update failed: ID {request.Id} not found.");
                    return ServiceResponse.Failure($"AcademicYear with ID {request.Id} not found.");
                }

                // 2. Apply updates from the command to the entity
                // The null-coalescing operator (??) correctly applies the update only if the request property is NOT null.
                academicYearToUpdate.Name = request.Name ?? academicYearToUpdate.Name;
                academicYearToUpdate.StartDate = request.StartDate ?? academicYearToUpdate.StartDate;
                academicYearToUpdate.EndDate = request.EndDate ?? academicYearToUpdate.EndDate;

                // 3. Update the repository (optional but good practice for clarity and tracking)
                // Note: Assuming UpdateAsync takes the entity, not the ID and entity.
                await repository.UpdateAsync(academicYearToUpdate.Id, academicYearToUpdate, cancellationToken);

                // 4. Commit transaction
                await unitOfWork.CommitAsync(cancellationToken);

                logger.LogInfo($"Successfully updated AcademicYear: {request.Id}");

                // Use ServiceResponse.Success
                return ServiceResponse.Success(
                    $"AcademicYear '{academicYearToUpdate.Name}' was successfully updated.",
                    new { academicYearToUpdate.Id, academicYearToUpdate.Name } // Return updated ID/Name
                );
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency conflicts
                logger.LogError(ex, $"Concurrency error during update of AcademicYear ID: {request.Id}");

                // Use ServiceResponse.Failure
                return ServiceResponse.Failure("The record you are trying to update has been modified by another user. Please refresh and try again.");
            }
            catch (Exception ex)
            {
                // Log all other unexpected errors
                logger.LogError(ex, $"Error occurred while updating AcademicYear ID: {request.Id}");

                // Use ServiceResponse.Failure
                return ServiceResponse.Failure("An unexpected error occurred during the AcademicYear update process. Please contact support.");
            }
        }
    }
}