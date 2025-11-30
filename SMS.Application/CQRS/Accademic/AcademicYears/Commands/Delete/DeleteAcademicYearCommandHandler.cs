using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Commands.Delete
{
    // Note: Renamed the class from DeleteClassesCommandHandler to DeleteAcademicYearCommandHandler for clarity
    public class DeleteAcademicYearCommandHandler(
        IAcademicYearRepository repository,
        IUnitOfWork unitOfWork,
        IAppLogger<DeleteAcademicYearCommandHandler> logger)
        : IRequestHandler<DeleteAcademicYearCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteAcademicYearCommand request, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Starting AcademicYear deletion for ID: {request.Id}");

            try
            {
                // 1. Retrieve the existing entity
                // Note: It's better to fetch the entity to ensure existence before attempting deletion, 
                // and to avoid issues if the DeleteAsync method expects a tracked entity.
                var academicYearToDelete = await repository.GetAsync(request.Id, cancellationToken);

                if (academicYearToDelete == null)
                {
                    logger.LogWarning($"AcademicYear deletion failed: ID {request.Id} not found.");

                    // Use ServiceResponse.Failure
                    return ServiceResponse.Failure($"AcademicYear with ID {request.Id} not found.");
                }

                // 2. Remove the entity
                await repository.DeleteAsync(academicYearToDelete.Id, cancellationToken); // Using the retrieved entity or ID

                // 3. Commit transaction
                int result = await unitOfWork.CommitAsync(cancellationToken);

                // Optional: Check if commit affected any rows
                if (result == 0)
                {
                    // This scenario is rare but handles cases where the entity might have been deleted concurrently
                    logger.LogWarning($"AcademicYear deletion failed: ID {request.Id} found but no records were affected during commit.");
                    return ServiceResponse.Failure($"AcademicYear deletion failed: No records were affected for ID {request.Id}.");
                }

                // 4. Success
                logger.LogInfo($"Successfully deleted AcademicYear with ID: {request.Id}");

                // Use ServiceResponse.Success
                return ServiceResponse.Success($"AcademicYear with ID {request.Id} was successfully deleted.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Critical error during AcademicYear deletion for ID: {request.Id}");

                // Use ServiceResponse.Failure for unhandled exceptions
                return ServiceResponse.Failure($"An unexpected error occurred during deletion of AcademicYear ID {request.Id}. Error: {ex.Message}");
            }
        }
    }
}