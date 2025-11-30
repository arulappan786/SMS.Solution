using AutoMapper;
using FluentValidation;
using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Application.Validations;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;


namespace SMS.Application.CQRS.Accademic.Classes.Commands.Create
{
    public class CreateClassesCommandHandler(IClassesRepository repository,
                                             IUnitOfWork unitOfWork,
                                             IValidationService validationService,
                                             IValidator<CreateClassesCommand> validator,
                                             IMapper mapper,
                                             IAppLogger<CreateClassesCommandHandler> logger) 
        : IRequestHandler<CreateClassesCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(CreateClassesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Validation
                logger.LogInfo($"Starting Class creation: Validating Input.");
                var validationResult = await validationService.ValidateAsync(request, validator);
                if (!validationResult.Succeeded)
                {
                    logger.LogWarning($"Class creation failed validation for {request.Name}.");
                    return validationResult;
                }

                // 2. Uniqueness Check (Application Business Rule)
                var existsResult = await repository.ExistsAsync(request.Name, cancellationToken);
                if (existsResult)
                {
                    logger.LogWarning($"Class creation failed: Duplicate entry detected for {request.Name}.");
                    //return ServiceResponse.Failure("An Class with this name or date range already exists in the system.");
                    return new ServiceResponse { Succeeded = false, Message = "An Class with this name already exists in the system." };
                }

                // 3. Mapping and Persistence
                var classess = mapper.Map<Domain.Entities.Academic.Classes>(request);

                await repository.AddAsync(classess, cancellationToken);

                // 4. Commit Transaction
                var result = await unitOfWork.CommitAsync(cancellationToken);

                // Optional but robust check: If 0 rows were affected, throw a specific exception.
                if (result <= 0)
                {
                    // This indicates a logical failure, not a database failure.
                    throw new InvalidOperationException($"Class creation failed for {request.Name}: No records were committed.");
                }

                // 5. Success
                logger.LogInfo($"Successfully created Class: {classess.Id} with name {request.Name}.");
                return new ServiceResponse { Succeeded = true, Message = $"New Class is created with the name: {request.Name}." };
            }
            catch (Exception ex)
            {
                // Log the exception details for developers/operations team
                logger.LogError(ex, $"Error while creating Class {request.Name} into the system.");

                // Return a clean, user-facing error response
                return new ServiceResponse { Succeeded = false, Message = $"An unexpected error occurred while creating the Class. Please try again or contact support." };
            }
        }
    }
}
