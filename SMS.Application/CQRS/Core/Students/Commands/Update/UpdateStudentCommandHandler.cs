using MediatR;
using Microsoft.EntityFrameworkCore;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Domain.Interfaces.Repositories.Common;
using SMS.Domain.Interfaces.Repositories.Core;

namespace SMS.Application.CQRS.Core.Students.Commands.Update
{
    public class UpdateStudentCommandHandler(IStudentRepository repository,
            IUnitOfWork unitOfWork,
            IAppLogger<UpdateStudentCommandHandler> logger) : IRequestHandler<UpdateStudentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInfo($"Starting student update for ID: {request.Id}");

                // 1. Retrieve the existing entity
                var studentToUpdate = await repository.GetAsync(request.Id, cancellationToken);

                if (studentToUpdate == null)
                {
                    return new ServiceResponse { Succeeded = false, Message = $"Student with ID {request.Id} not found."};
                }

                // 2. Apply updates from the command to the entity
                // Use the entity methods to trigger the state change
                studentToUpdate.UpdatePersonalInfo(request.FullName ?? studentToUpdate.FullName,
                                                   request.DateOfBirth ?? studentToUpdate.DateOfBirth,
                                                   request.Gender ?? studentToUpdate.Gender,
                                                   request.Email ?? studentToUpdate.Email);
                studentToUpdate.ChangeAddress(request.HomeAddress ?? studentToUpdate.HomeAddress);

                // 3. Update the repository (often unnecessary if entity is tracked, but good practice)
                await repository.UpdateAsync(request.Id, studentToUpdate, cancellationToken);

                // 4. Commit transaction
                await unitOfWork.CommitAsync(cancellationToken);

                logger.LogInfo($"Successfully updated student: {request.Id}");

                return new ServiceResponse { Succeeded = true, Message = $"Student '{request.FullName}' was successfully updated." };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency conflicts (e.g., another user updated the record simultaneously)
                logger.LogError(ex, $"Concurrency error during update of Student ID: {request.Id}");
                return new ServiceResponse { Succeeded = false, Message = $"The record you are trying to update has been modified by another user. Please refresh and try again." };
            }
            catch (Exception ex)
            {
                // Log all other unexpected errors
                logger.LogError(ex, $"Error occurred while updating Student ID: {request.Id}");
                return new ServiceResponse { Succeeded = false, Message = $"An unexpected error occurred during the student update process." };
            }
        }
    }
}
