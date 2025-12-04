using MediatR;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.CQRS.Identity.Logins.Commands;

namespace SMS.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticationController(IMediator mediator) : ControllerBase
    {
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
    }
}
