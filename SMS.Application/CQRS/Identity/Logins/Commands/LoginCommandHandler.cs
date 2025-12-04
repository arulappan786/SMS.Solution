using FluentValidation;
using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Identity;
using SMS.Application.Services.Logging;

namespace SMS.Application.CQRS.Identity.Logins.Commands
{
    public class LoginCommandHandler(IValidator<LoginCommand> validator,
                                     IIdentityService identityService,
                                     IAppLogger<LoginCommandHandler> logger) : IRequestHandler<LoginCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // --- 1. Validation (As provided) ---
            logger.LogInfo($"Starting user login: Validating Input.");

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                logger.LogWarning($"Login validation failed validation for {request.EmailAddess}.");

                // Return failure response with detailed validation errors
                return ServiceResponse.Failure(
                    "Validation failed for the login command.",
                    validationResult.Errors);
            }

            // --- 2. Core Authentication Logic ---
            logger.LogInfo($"Input validated for {request.EmailAddess}. Attempting authentication.");

            var (success, message, userDto) = await identityService.LoginAsync(
                request.EmailAddess,
                request.Password,
                cancellationToken);

            // --- 3. Handle Authentication Result ---
            if (success)
            {
                logger.LogInfo($"User {request.EmailAddess} logged in successfully. UserID: {userDto.UserId}.");

                // Successful login returns the LoggedInUserDto as the Data payload
                return ServiceResponse.Success(
                    "User logged in successfully.",
                    data: userDto);
            }
            else
            {
                logger.LogWarning($"Login failed for user {request.EmailAddess}. Reason: {message}");

                // Authentication failed (e.g., bad credentials, locked account)
                return ServiceResponse.Failure(message);
            }
        }
    }
}
