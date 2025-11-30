using AutoMapper;
using FluentValidation;
using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Application.Validations;
using SMS.Domain.Entities.Academic;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Commands.Create
{
    public class CreateAcademicYearCommandHandler(IAcademicYearRepository repository,
                                                  IUnitOfWork unitOfWork,
                                                  IValidationService validationService,
                                                  IValidator<CreateAcademicYearCommand> validator,
                                                  IMapper mapper,
                                                  IAppLogger<CreateAcademicYearCommandHandler> logger) 
        : IRequestHandler<CreateAcademicYearCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(CreateAcademicYearCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Validation
                logger.LogInfo($"Starting Academic Year creation: Validating Input.");
                var validationResult = await validationService.ValidateAsync(request, validator);
                if (!validationResult.Succeeded)
                {
                    logger.LogWarning($"Academic Year creation failed validation for {request.Name}.");
                    return validationResult;
                }

                // 2. Uniqueness Check (Application Business Rule)
                var existsResult = await repository.ExistsAsync(request.Name, request.StartDate, request.EndDate, cancellationToken);
                if (existsResult)
                {
                    logger.LogWarning($"Academic Year creation failed: Duplicate entry detected for {request.Name}.");
                    //return ServiceResponse.Failure("An Academic Year with this name or date range already exists in the system.");
                    return new ServiceResponse { Succeeded = false, Message = "An Academic Year with this name or date range already exists in the system." };
                }

                // 3. Mapping and Persistence
                var academicYear = mapper.Map<AcademicYear>(request);

                await repository.AddAsync(academicYear, cancellationToken);

                // 4. Commit Transaction
                var result = await unitOfWork.CommitAsync(cancellationToken);

                // Optional but robust check: If 0 rows were affected, throw a specific exception.
                if (result <= 0)
                {
                    // This indicates a logical failure, not a database failure.
                    throw new InvalidOperationException($"Academic Year creation failed for {request.Name}: No records were committed.");
                }

                // 5. Success
                logger.LogInfo($"Successfully created Academic Year: {academicYear.Id} with name {request.Name}.");
                return new ServiceResponse { Succeeded = true, Message = $"New Academic Year is created with the name: {request.Name}." };
            }
            catch (Exception ex)
            {
                // Log the exception details for developers/operations team
                logger.LogError(ex, $"Error while creating Academic Year {request.Name} into the system.");

                // Return a clean, user-facing error response
                return new ServiceResponse { Succeeded = false, Message = $"An unexpected error occurred while creating the Academic Year. Please try again or contact support." };
            }
        }
    }
}
