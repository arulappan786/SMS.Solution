using FluentValidation;
using SMS.Application.DTOs.Service;

namespace SMS.Application.Validations
{
    public class ValidationService : IValidationService
    {
        public async Task<ServiceResponse> ValidateAsync<T>(T model, IValidator<T> validator)
        {
            var _validation = await validator.ValidateAsync(model);

            if (!_validation.IsValid)
            {
                var errors = _validation.Errors.Select(e => e.ErrorMessage).ToList();
                var errorMessage = string.Join("; ", errors);
                return new ServiceResponse { Succeeded = false, Message = errorMessage };
            }

            return new ServiceResponse { Succeeded = true, Message = "Validation successful" };
        }
    }
}