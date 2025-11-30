using AutoMapper;
using FluentValidation;
using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Application.CQRS.Accademic.Classes.Commands.Create
{
    public class CreateClassesCommandHandler(IClassesRepository repository,
                                             IUnitOfWork unitOfWork,
                                             IValidator<CreateClassesCommand> validator,
                                             IMapper mapper,
                                             IAppLogger<CreateClassesCommandHandler> logger)
        : IRequestHandler<CreateClassesCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(CreateClassesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // --- 1. Validation ---
                logger.LogInfo($"Starting Class creation: Validating Input.");

                // Use the IValidator directly for validation
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    logger.LogWarning($"Class creation failed validation for {request.Name}.");

                    // Use ServiceResponse.Failure and pass validation errors as Data
                    return ServiceResponse.Failure(
                        "Validation failed for the Class command.",
                        validationResult.Errors);
                }

                // --- 2. Uniqueness Check (Application Business Rule) ---
                var existsResult = await repository.ExistsAsync(request.Name, request.AcademicYearId, cancellationToken);

                if (existsResult)
                {
                    logger.LogWarning($"Class creation failed: Duplicate entry detected for {request.Name}.");

                    // Use ServiceResponse.Failure for business rule violation
                    return ServiceResponse.Failure("A Class with this name already exists in the system.");
                }

                // --- 3. Mapping and Persistence ---
                // Removed redundant Domain.Entities.Academic namespace prefix
                var newClass = mapper.Map<Domain.Entities.Academic.Classes>(request);

                await repository.AddAsync(newClass, cancellationToken);

                // --- 4. Commit Transaction ---
                var result = await unitOfWork.CommitAsync(cancellationToken);

                // Check for logical failure 
                if (result <= 0)
                {
                    logger.LogError($"Class creation failed for {request.Name}: No records were committed.");
                    // Use ServiceResponse.Failure
                    return ServiceResponse.Failure($"Class creation failed. No records were committed for {request.Name}.");
                }

                // --- 5. Success ---
                logger.LogInfo($"Successfully created Class: {newClass.Id} with name {request.Name}.");

                // Use ServiceResponse.Success
                return ServiceResponse.Success(
                    $"New Class is created with the name: {request.Name}.",
                    new { ClassId = newClass.Id, newClass.Name } // Return created ID/Name
                );
            }
            catch (Exception ex)
            {
                // --- 6. Centralized Error Handling ---
                logger.LogError(ex, $"Error while creating Class {request.Name} into the system.");

                // Use ServiceResponse.Failure for unhandled exceptions
                return ServiceResponse.Failure($"An unexpected error occurred while creating the Class. Error: {ex.Message}");
            }
        }
    }
}