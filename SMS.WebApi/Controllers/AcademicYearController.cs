using MediatR;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.CQRS.Accademic.AcademicYears.Commands;
using SMS.Application.CQRS.Core.Students.Commands.CreateStudent;
using SMS.Application.DTOs.Service;
using System.Net;

namespace SMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicYearController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAcademicYearCommand command)
        {
            var serviceResponse = await mediator.Send(command);

            if (serviceResponse.Success)
            {
                // Consistent: Return 201 Created with the ServiceResponse.
                return StatusCode((int)HttpStatusCode.Created, serviceResponse);
            }
            else
            {
                // Consistent: Return 400 Bad Request with the ServiceResponse.
                return BadRequest(serviceResponse);
            }
        }
    }
}
