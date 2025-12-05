using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.CQRS.Identity.Logins.Commands;
using SMS.Application.CQRS.Identity.Registers.Commands;
using System.Security.Claims;

namespace SMS.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController(IMediator mediator) : ControllerBase
    {

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="requestDto">The registration data including username, email, and passwords.</param>
        [HttpPost("register")]
        // This endpoint MUST be anonymous (no [Authorize] needed)
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            // 1. Send Command through MediatR
            var response = await mediator.Send(command);

            if (response.Succeeded)
            {
                // Return 201 Created Status
                return StatusCode(201, response.Message);
            }

            // Return 400 Bad Request for validation or business logic failure
            return BadRequest(response.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var serviceResponse = await mediator.Send(command);

            if (serviceResponse.Succeeded)
            {
                return Ok(serviceResponse);
            }
            else
            {
                return BadRequest(serviceResponse);
            }
        }

        [HttpPost("refreshtoken")]
        // This endpoint should NOT require [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            // Send the command through MediatR
            var response = await mediator.Send(command);

            if (response.Succeeded)
            {
                // Return the new tokens (LoggedInUserDto) and a 200 OK
                return Ok(response);
            }

            // Return 401 Unauthorized if the refresh token is invalid/expired
            return Unauthorized(response.Message);
        }

        [Authorize] // Must be a valid, logged-in user to revoke their own token
        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeTokens()
        {
            // 1. Safely extract the User ID from the active JWT claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                // Indicates a corrupted token or identity setup
                return Unauthorized("Invalid user identity in token.");
            }

            // 2. Create the MediatR Command
            var command = new RevokeTokensCommand(userId);

            // 3. Send the command and receive the ServiceResponse
            var response = await mediator.Send(command);

            if (response.Succeeded)
            {
                // 204 No Content is appropriate for a successful state change/invalidation
                return NoContent();
            }

            // Return 400 Bad Request or 401 Unauthorized based on failure reason
            return BadRequest(response.Message);
        }

        [HttpPost("revoke/{targetUserId}")]
        public async Task<IActionResult> ForceRevokeUserTokens(Guid targetUserId)
        {
            // 1. Get the ID of the ADMIN performing the action (for auditing)
            var adminUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(adminUserIdClaim, out var adminUserId))
            {
                return Unauthorized("Admin identity could not be determined.");
            }

            // 2. Create the Admin Revocation Command
            var command = new AdminRevokeTokensCommand(
                TargetUserId: targetUserId,
                AdminUserId: adminUserId
            );

            // 3. Send the command
            var response = await mediator.Send(command);

            if (response.Succeeded)
            {
                return NoContent(); // Success, session terminated
            }

            // Return 404 if the user ID wasn't found, or 400 for other failures
            return NotFound(response.Message);
        }

        /// <summary>
        /// Logs out the currently authenticated user by revoking their Refresh Token.
        /// </summary>
        /// <returns>204 No Content on successful revocation.</returns>
        [Authorize] // Requires a valid Access Token to identify the user
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // 1. Safely extract the User ID from the active JWT claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                // Should not happen if [Authorize] passes, but guards against malformed claim
                return Unauthorized("Invalid user identity in token.");
            }

            // 2. Create and Send the Revocation Command
            // This reuses the logic defined in RevokeTokensCommandHandler
            var command = new RevokeTokensCommand(userId);
            var response = await mediator.Send(command);

            if (response.Succeeded)
            {
                // Client must also clear its locally stored Access Token and Refresh Token.
                return NoContent(); // HTTP 204: Success, but no data to return
            }

            // If the command failed (e.g., database error), return appropriate status
            return StatusCode(500, response.Message);
        }
    }
}
