using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Commands.DeleteAcademicYear
{
    public class DeleteAcademicYearCommandHandler(IAcademicYearRepository repository,
            IUnitOfWork unitOfWork,
            IAppLogger<DeleteAcademicYearCommandHandler> logger) : IRequestHandler<DeleteAcademicYearCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteAcademicYearCommand request, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Starting AcademicYear deletion for ID: {request.Id}");

            // 1. Retrieve the existing entity by ID
            var studentToDelete = await repository.GetAsync(request.Id, cancellationToken);

            if (studentToDelete == null)
            {
                logger.LogInfo($"Starting AcademicYear deletion for ID: {request.Id}");
                return new ServiceResponse { Success = false, Message = $"Student with ID {request.Id} not found." };
            }

            // 2. Remove the entity
             await repository.DeleteAsync(request.Id, cancellationToken);

            // 3. Commit transaction
            await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInfo($"Starting AcademicYear deletion for ID: {request.Id}");

            return new ServiceResponse { Success = false, Message = $"Student with ID {request.Id} was successfully deleted." };
        }
    }
}
