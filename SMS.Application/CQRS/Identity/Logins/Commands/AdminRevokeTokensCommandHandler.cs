using MediatR;
using SMS.Application.DTOs.Service;
using SMS.Application.Services.Identity;
using SMS.Application.Services.Logging;

namespace SMS.Application.CQRS.Identity.Logins.Commands
{
    public class AdminRevokeTokensCommandHandler(
        IUserManagementService userManagementService,
        IAppLogger<AdminRevokeTokensCommandHandler> logger) : IRequestHandler<AdminRevokeTokensCommand, ServiceResponse>
    {
        public async Task<ServiceResponse> Handle(AdminRevokeTokensCommand request, CancellationToken cancellationToken)
        {
            logger.LogWarning($"ADMIN REVOCATION initiated by User {request.AdminUserId} for target User {request.TargetUserId}");

            // Delegation: Call the persistence service to clear the token fields
            var (success, message) = await userManagementService.RevokeUserTokensAsync(request.TargetUserId);

            if (success)
            {
                logger.LogWarning($"ADMIN REVOCATION successful for target User {request.TargetUserId}.");
                return ServiceResponse.Success(message);
            }
            else
            {
                logger.LogError($"ADMIN REVOCATION failed for target User {request.TargetUserId}. Reason: {message}");
                return ServiceResponse.Failure(message);
            }
        }
    }
}