using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Identity;
using SMS.Application.Services.Logging;

namespace SMS.Application.CQRS.Identity.Logins.Commands
{
    // This handler orchestrates the token invalidation.
    public class RevokeTokensCommandHandler(
        IUserManagementService userManagementService, // Dependency for persistence
        IAppLogger<RevokeTokensCommandHandler> logger) : IRequestHandler<RevokeTokensCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(RevokeTokensCommand request, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Attempting to revoke tokens for user ID: {request.UserId}");

            // Delegation: Call the persistence service
            var (success, message) = await userManagementService.RevokeUserTokensAsync(request.UserId);

            if (success)
            {
                logger.LogInfo($"Tokens successfully revoked for user ID: {request.UserId}");
                return ServiceResponse.Success(message);
            }
            else
            {
                logger.LogError($"Token revocation failed for user ID: {request.UserId}. Reason: {message}");
                return ServiceResponse.Failure(message);
            }
        }
    }
}
