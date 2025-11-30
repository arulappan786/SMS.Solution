using MediatR;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.CQRS.Accademic.Classes.Commands.Create;
using SMS.Application.CQRS.Accademic.Classes.Commands.Delete;
using SMS.Application.CQRS.Accademic.Classes.Commands.Update;
using SMS.Application.CQRS.Accademic.Classes.Queries.GetAll;
using SMS.Application.CQRS.Accademic.Classes.Queries.GetById;
using SMS.Application.DTOs.Accademic.Classes;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Service;
using System.Net;

[Route("api/[controller]")]
[ApiController]
public class ClassController(IMediator mediator) : ControllerBase
{
    // --- QUERY: Get All Classs ---

    [HttpGet]
    // Assume handler returns PaginatedResultDto (empty list if none found, never null)
    [ProducesResponseType(typeof(PaginatedResultDto<ClassesDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllClasssesQuery query)
    {
        var students = await mediator.Send(query);
        // Clean: Always returns 200 OK, with an empty list if no records exist.
        return Ok(students);
    }

    // --- QUERY: Get Class by ID ---

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClassesDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)] // Best handled by global filter
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetClassesByIdQuery { Id = id };
        var student = await mediator.Send(query);

        // If global exception handling is NOT used, keep this check:
        if (student == null) return NotFound();

        return Ok(student);
    }

    // --- COMMAND: Create Class ---

    [HttpPost]
    [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateClassesCommand command)
    {
        var serviceResponse = await mediator.Send(command);

        if (serviceResponse.Succeeded)
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

    // --- COMMAND: Update Class ---

    [HttpPut("{id}")] // Use HTTP PUT for full resource replacement/update
    [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.NotFound)] // Added for clarity
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClassesCommand command)
    {
        // 1. Ensure the ID in the route matches the ID in the command body
        if (id != command.Id)
        {
            return BadRequest(new ServiceResponse { Succeeded = false, Message = "ID mismatch: The ID in the route does not match the ID in the request body." });
        }

        // 2. Send the command to MediatR
        var serviceResponse = await mediator.Send(command);

        if (serviceResponse.Succeeded)
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


    // --- COMMAND: Delete Class ---

    [HttpDelete("{id}")] // Use HTTP DELETE
    [ProducesResponseType((int)HttpStatusCode.NoContent)] // 204 No Content is standard for successful deletion
    [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteClassesCommand { Id = id };

        var serviceResponse = await mediator.Send(command);

        if (serviceResponse.Succeeded)
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