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
                                             IAppLogger<UpdateStudentCommandHandler> logger)
        : IRequestHandler<UpdateStudentCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Starting student update for ID: {request.Id}");

            try
            {
                // 1. Retrieve the existing entity
                var studentToUpdate = await repository.GetAsync(request.Id, cancellationToken);

                if (studentToUpdate == null)
                {
                    logger.LogWarning($"Student update failed: ID {request.Id} not found.");
                    // Use ServiceResponse.Failure for Not Found
                    return ServiceResponse.Failure($"Student with ID {request.Id} not found.");
                }

                // 2. Apply updates from the command to the entity
                // The null-coalescing operator (??) ensures properties are updated only if provided in the request.
                // NOTE: This assumes FullName, DateOfBirth, Gender, and Email are nullable in the UpdateStudentCommand.
                studentToUpdate.UpdatePersonalInfo(
                    request.FullName ?? studentToUpdate.FullName,
                    request.DateOfBirth ?? studentToUpdate.DateOfBirth,
                    request.Gender ?? studentToUpdate.Gender,
                    request.Email ?? studentToUpdate.Email);

                studentToUpdate.ChangeAddress(request.HomeAddress ?? studentToUpdate.HomeAddress);

                // 3. Update the repository (optional but good practice)
                // Note: Changed to pass the entity as the repository should handle tracking efficiently.
                await repository.UpdateAsync(studentToUpdate.Id, studentToUpdate, cancellationToken);

                // 4. Commit transaction
                await unitOfWork.CommitAsync(cancellationToken);

                logger.LogInfo($"Successfully updated student: {request.Id}");

                // Use ServiceResponse.Success
                return ServiceResponse.Success(
                    $"Student '{studentToUpdate.FullName.FirstName} {studentToUpdate.FullName.LastName}' was successfully updated.",
                    new { StudentId = studentToUpdate.Id } // Return updated ID
                );
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency conflicts
                logger.LogError(ex, $"Concurrency error during update of Student ID: {request.Id}");

                // Use ServiceResponse.Failure
                return ServiceResponse.Failure("The record you are trying to update has been modified by another user. Please refresh and try again.");
            }
            catch (Exception ex)
            {
                // Log all other unexpected errors
                logger.LogError(ex, $"Error occurred while updating Student ID: {request.Id}");

                // Use ServiceResponse.Failure
                return ServiceResponse.Failure("An unexpected error occurred during the student update process. Please contact support.");
            }
        }
    }
}