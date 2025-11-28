using AutoMapper;
using FluentValidation;
using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Logging;
using SMS.Application.Validations;
using SMS.Domain.Entities.Academic;
using SMS.Domain.Interfaces.Repositories.Academic;
using SMS.Domain.Interfaces.Repositories.Common;

namespace SMS.Application.CQRS.Accademic.AcademicYears.Commands
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
                logger.LogInfo($"Starting Accademic Year creation: Validating Input");
                var validationResult = await validationService.ValidateAsync(request, validator);
                if (!validationResult.Success) return validationResult;

                var mapped = mapper.Map<AcademicYear>(request);

                // Adding the student to the student store.
                await repository.AddAsync(mapped, cancellationToken);

                // Commiting the identity and student transactions.
                var result = await unitOfWork.CommitAsync(cancellationToken);

                if (result <= 0)
                    throw new InvalidOperationException($"Accademic Year record creation failed for {request.Name} during final commit.");

                return new ServiceResponse()
                {
                    Success = true,
                    Message = $"New Accademic Year is created with the name: {request.Name}."
                };
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "Error while creating Accademic Year into the system.");

                return new ServiceResponse()
                {
                    Success = false,
                    Message = $"Error while Accademic Year into the system. {ex.Message}"
                };
            }            
        }
    }
}
