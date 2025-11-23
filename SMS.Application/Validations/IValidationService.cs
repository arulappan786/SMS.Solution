using FluentValidation;
using SMS.Application.DTOs.Service;

namespace SMS.Application.Validations
{
    public interface IValidationService
    {
        Task<ServiceResponse> ValidateAsync<T>(T model, IValidator<T> validator);
    }
}
