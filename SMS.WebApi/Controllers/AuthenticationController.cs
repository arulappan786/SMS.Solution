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
    }
}
