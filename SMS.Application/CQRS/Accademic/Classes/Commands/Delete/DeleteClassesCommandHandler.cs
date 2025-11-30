using MediatR;
using SMS.Application.CQRS.Accademic.Classes.Commands.Delete;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Application.CQRS.Accademic.Classess.Commands.Delete
{
    public class DeleteClassesCommandHandler(IClassesRepository repository,
            IUnitOfWork unitOfWork,
            IAppLogger<DeleteClassesCommandHandler> logger) : IRequestHandler<DeleteClassesCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(DeleteClassesCommand request, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Starting Class deletion for ID: {request.Id}");

            // 1. Retrieve the existing entity by ID
            var studentToDelete = await repository.GetAsync(request.Id, cancellationToken);

            if (studentToDelete == null)
            {
                logger.LogInfo($"Starting Class deletion for ID: {request.Id}");
                return new ServiceResponse { Success = false, Message = $"Class with ID {request.Id} not found." };
            }

            // 2. Remove the entity
             await repository.DeleteAsync(request.Id, cancellationToken);

            // 3. Commit transaction
            await unitOfWork.CommitAsync(cancellationToken);

            logger.LogInfo($"Starting Class deletion for ID: {request.Id}");

            return new ServiceResponse { Success = false, Message = $"Class with ID {request.Id} was successfully deleted." };
        }
    }
}
