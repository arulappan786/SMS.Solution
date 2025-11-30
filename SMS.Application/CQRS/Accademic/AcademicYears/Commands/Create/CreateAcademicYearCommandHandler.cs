using AutoMapper;
using FluentValidation;
using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Domain.Entities.Academic;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Commands.Create
{
    public class CreateAcademicYearCommandHandler(IAcademicYearRepository repository,
                                                  IUnitOfWork unitOfWork,
                                                  IValidator<CreateAcademicYearCommand> validator,
                                                  IMapper mapper,
                                                  IAppLogger<CreateAcademicYearCommandHandler> logger)
        : IRequestHandler<CreateAcademicYearCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(CreateAcademicYearCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // --- 1. Validation ---
                logger.LogInfo($"Starting Academic Year creation: Validating Input.");

                // Use the IValidator directly to perform validation
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    logger.LogWarning($"Academic Year creation failed validation for {request.Name}.");

                    // Return failure response with detailed validation errors
                    return ServiceResponse.Failure(
                        "Validation failed for the Academic Year command.",
                        validationResult.Errors); // Passing errors as Data
                }

                // --- 2. Uniqueness Check (Application Business Rule) ---
                var existsResult = await repository.ExistsAsync(request.Name, request.StartDate, request.EndDate, cancellationToken);

                if (existsResult)
                {
                    logger.LogWarning($"Academic Year creation failed: Duplicate entry detected for {request.Name}.");

                    // Use ServiceResponse.Failure for business rule violation
                    return ServiceResponse.Failure("An Academic Year with this name or date range already exists in the system.");
                }

                // --- 3. Mapping and Persistence ---
                var academicYear = mapper.Map<AcademicYear>(request);

                await repository.AddAsync(academicYear, cancellationToken);

                // --- 4. Commit Transaction ---
                var result = await unitOfWork.CommitAsync(cancellationToken);

                // Check for logical failure (though EF usually throws an exception here if the underlying context fails)
                if (result <= 0)
                {
                    logger.LogError($"Academic Year creation failed for {request.Name}: No records were committed.");
                    return ServiceResponse.Failure($"Academic Year creation failed. No records were committed for {request.Name}.");
                }

                // --- 5. Success ---
                logger.LogInfo($"Successfully created Academic Year: {academicYear.Id} with name {request.Name}.");

                // Use ServiceResponse.Success
                return ServiceResponse.Success(
                    $"New Academic Year is created with the name: {request.Name}.",
                    new { AcademicYearId = academicYear.Id, Name = academicYear.Name } // Optional: return new ID
                );
            }
            catch (Exception ex)
            {
                // --- 6. Centralized Error Handling ---
                logger.LogError(ex, $"Error while creating Academic Year {request.Name} into the system.");

                // Return a clean, user-facing error response using ServiceResponse.Failure
                return ServiceResponse.Failure($"An unexpected error occurred while creating the Academic Year. Error: {ex.Message}");
            }
        }
    }
}