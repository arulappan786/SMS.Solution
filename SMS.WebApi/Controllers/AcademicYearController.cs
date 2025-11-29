using MediatR;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.CQRS.Accademic.AcademicYears.Commands.CreateAcademicYear;
using SMS.Application.CQRS.Accademic.AcademicYears.Commands.DeleteAcademicYear;
using SMS.Application.CQRS.Accademic.AcademicYears.Commands.UpdateAcademicYear;
using SMS.Application.CQRS.Accademic.AcademicYears.Queries.GetAcademicYearById;
using SMS.Application.CQRS.Accademic.AcademicYears.Queries.GetAllAcademicYearList;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Accademic.AcademicYears;
using SMS.Application.DTOs.Service;
using System.Net;

[Route("api/[controller]")]
[ApiController]
public class AcademicYearController(IMediator mediator) : ControllerBase
{
    // --- QUERY: Get All AcademicYears ---

    [HttpGet]
    // Assume handler returns PaginatedResultDto (empty list if none found, never null)
    [ProducesResponseType(typeof(PaginatedResultDto<AcademicYearDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllAcademicYearsQuery query)
    {
        var students = await mediator.Send(query);
        // Clean: Always returns 200 OK, with an empty list if no records exist.
        return Ok(students);
    }

    // --- QUERY: Get AcademicYear by ID ---

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AcademicYearDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)] // Best handled by global filter
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetAcademicYearByIdQuery { Id = id };
        var student = await mediator.Send(query);

        // If global exception handling is NOT used, keep this check:
        if (student == null) return NotFound();

        return Ok(student);
    }

    // --- COMMAND: Create AcademicYear ---

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

    // --- COMMAND: Update AcademicYear ---

    [HttpPut("{id}")] // Use HTTP PUT for full resource replacement/update
    [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.NotFound)] // Added for clarity
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAcademicYearCommand command)
    {
        // 1. Ensure the ID in the route matches the ID in the command body
        if (id != command.Id)
        {
            return BadRequest(new ServiceResponse { Success = false, Message = "ID mismatch: The ID in the route does not match the ID in the request body." });
        }

        // 2. Send the command to MediatR
        var serviceResponse = await mediator.Send(command);

        if (serviceResponse.Success)
        {
            // Consistent: Return 200 OK on successful update.
            return Ok(serviceResponse);
        }
        else
        {
            // Check for NotFound message from the handler
            if (serviceResponse.Message.Contains("not found", System.StringComparison.OrdinalIgnoreCase))
            {
                // Return 404 Not Found if the record doesn't exist.
                return NotFound(serviceResponse);
            }

            // Consistent: Return 400 Bad Request for validation or other business rule failures.
            return BadRequest(serviceResponse);
        }
    }


    // --- COMMAND: Delete AcademicYear ---

    [HttpDelete("{id}")] // Use HTTP DELETE
    [ProducesResponseType((int)HttpStatusCode.NoContent)] // 204 No Content is standard for successful deletion
    [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteAcademicYearCommand { Id = id };

        var serviceResponse = await mediator.Send(command);

        if (serviceResponse.Success)
        {
            // Standard REST practice: 204 No Content on successful deletion
            return NoContent();
        }
        else
        {
            // Check for NotFound message from the handler
            if (serviceResponse.Message.Contains("not found", System.StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(serviceResponse);
            }

            // Return 400 Bad Request for validation errors or unexpected failures
            return BadRequest(serviceResponse);
        }
    }
}