using MediatR;
using Microsoft.EntityFrameworkCore;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Commands.Update
{
    public class UpdateClassesCommandHandler(IAcademicYearRepository repository,
            IUnitOfWork unitOfWork,
            IAppLogger<UpdateClassesCommandHandler> logger) : IRequestHandler<UpdateAcademicYearCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateAcademicYearCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInfo($"Starting AcademicYear update for ID: {request.Id}");

                // 1. Retrieve the existing entity
                var accademicYearToUpdate = await repository.GetAsync(request.Id, cancellationToken);

                if (accademicYearToUpdate == null)
                {
                    return new ServiceResponse { Success = false, Message = $"AccademicYear with ID {request.Id} not found."};
                }

                // 2. Apply updates from the command to the entity
                // Use the entity methods to trigger the state change
                accademicYearToUpdate.Name = request.Name ?? accademicYearToUpdate.Name;
                accademicYearToUpdate.StartDate = request.StartDate ?? accademicYearToUpdate.StartDate;
                accademicYearToUpdate.EndDate = request.EndDate ?? accademicYearToUpdate.EndDate;

                // 3. Update the repository (often unnecessary if entity is tracked, but good practice)
                await repository.UpdateAsync(request.Id, accademicYearToUpdate, cancellationToken);

                // 4. Commit transaction
                await unitOfWork.CommitAsync(cancellationToken);

                logger.LogInfo($"Successfully updated AcademicYear: {request.Id}");

                return new ServiceResponse { Success = true, Message = $"Student '{request.Name}' was successfully updated." };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency conflicts (e.g., another user updated the record simultaneously)
                logger.LogError(ex, $"Concurrency error during update of AccademicYear ID: {request.Id}");
                return new ServiceResponse { Success = false, Message = $"The record you are trying to update has been modified by another user. Please refresh and try again." };
            }
            catch (Exception ex)
            {
                // Log all other unexpected errors
                logger.LogError(ex, $"Error occurred while updating Student ID: {request.Id}");
                return new ServiceResponse { Success = false, Message = $"An unexpected error occurred during the AcademicYear update process." };
            }
        }
    }
}
