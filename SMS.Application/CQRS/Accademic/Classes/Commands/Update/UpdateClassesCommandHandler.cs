using MediatR;
using Microsoft.EntityFrameworkCore;
using SMS.Application.CQRS.Accademic.Classes.Commands.Update;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Application.CQRS.Accademic.Classess.Commands.Update
{
    public class UpdateClassesCommandHandler(IClassesRepository repository,
            IUnitOfWork unitOfWork,
            IAppLogger<UpdateClassesCommandHandler> logger) : IRequestHandler<UpdateClassesCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateClassesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInfo($"Starting Classes update for ID: {request.Id}");

                // 1. Retrieve the existing entity
                var classesToUpdate = await repository.GetAsync(request.Id, cancellationToken);

                if (classesToUpdate == null)
                {
                    return new ServiceResponse { Succeeded = false, Message = $"Classes with ID {request.Id} not found."};
                }

                // 2. Apply updates from the command to the entity
                // Use the entity methods to trigger the state change
                classesToUpdate.Name = request.Name ?? classesToUpdate.Name;
                classesToUpdate.AcademicYearId = request.AcademicYearId ?? classesToUpdate.AcademicYearId;
                classesToUpdate.MaxCapacity = request.MaxCapacity ?? classesToUpdate.MaxCapacity;

                // 3. Update the repository (often unnecessary if entity is tracked, but good practice)
                await repository.UpdateAsync(request.Id, classesToUpdate, cancellationToken);

                // 4. Commit transaction
                await unitOfWork.CommitAsync(cancellationToken);

                logger.LogInfo($"Successfully updated Classes: {request.Id}");

                return new ServiceResponse { Succeeded = true, Message = $"Student '{request.Name}' was successfully updated." };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency conflicts (e.g., another user updated the record simultaneously)
                logger.LogError(ex, $"Concurrency error during update of Classes ID: {request.Id}");
                return new ServiceResponse { Succeeded = false, Message = $"The record you are trying to update has been modified by another user. Please refresh and try again." };
            }
            catch (Exception ex)
            {
                // Log all other unexpected errors
                logger.LogError(ex, $"Error occurred while updating Student ID: {request.Id}");
                return new ServiceResponse { Succeeded = false, Message = $"An unexpected error occurred during the Classes update process." };
            }
        }
    }
}
