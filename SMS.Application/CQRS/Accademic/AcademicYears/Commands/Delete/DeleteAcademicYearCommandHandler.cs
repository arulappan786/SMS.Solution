using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;
using SMS.Application.Exceptions;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Commands.Delete
{
    public class DeleteAcademicYearCommandHandler(IAcademicYearRepository repository,
                                                  IUnitOfWork unitOfWork,
                                                  IAppLogger<DeleteAcademicYearCommandHandler> logger)
        : IRequestHandler<DeleteAcademicYearCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteAcademicYearCommand request, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Starting AcademicYear deletion for ID: {request.Id}");

            // **1. Retrieve the existing entity**
            var academicYearToDelete = await repository.GetAsync(request.Id, cancellationToken);

            if (academicYearToDelete == null)
            {
                logger.LogWarning($"AcademicYear deletion failed: ID {request.Id} not found.");

                // **Throw the specific exception so the API Filter can map it to 404**
                throw new EntityNotFoundException(nameof(academicYearToDelete), request.Id);
            }

            // **2. Remove the entity**
            // Note: If DeleteAsync takes an ID, it should handle the case where the entity might not exist,
            // but fetching first (above) is still good practice for existence checks.
            await repository.DeleteAsync(academicYearToDelete.Id);

            // **3. Commit transaction**
            // The DbUpdateException (Foreign Key violation) will be thrown here if CommitAsync fails.
            int result = await unitOfWork.CommitAsync(cancellationToken);

            // **4. Success**
            logger.LogInfo($"Successfully deleted AcademicYear with ID: {request.Id}");

            // Use ServiceResponse.Success
            return ServiceResponse.Success($"AcademicYear with ID {request.Id} was successfully deleted.");

            // NOTE ON CONCURRENCY:
            // If the entity was concurrently deleted (result == 0), EF Core's default
            // optimistic concurrency check would typically throw a DbUpdateConcurrencyException.
            // If your UnitOfWork/Repository doesn't enforce concurrency and just returns 0,
            // you might want to throw a specific exception here (e.g., new ConcurrencyException(...))
            // that your API filter can catch and map to 409 Conflict. For simplicity, we
            // rely on the DbUpdateException handler for database conflicts.
        }
    }
}