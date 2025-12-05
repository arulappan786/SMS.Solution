using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.Constants;
using SMS.Application.CQRS.Accademic.Classes.Commands.Create;
using SMS.Application.CQRS.Accademic.Classes.Commands.Delete;
using SMS.Application.CQRS.Accademic.Classes.Commands.Update;
using SMS.Application.CQRS.Accademic.Classes.Queries.GetAll;
using SMS.Application.CQRS.Accademic.Classes.Queries.GetById;
using SMS.Application.DTOs.Accademic.Classes;
using SMS.Application.DTOs.Service;
using System.Net;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.Admin)]
public class ClassController(IMediator mediator) : ControllerBase
{
    // --- QUERY: Get All Classes --- 🧑‍🏫

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllClassesQuery query)
    {
        var classes = await mediator.Send(query);
        return Ok(classes);
    }

    // --- QUERY: Get Class by ID ---

    [HttpGet("{id:guid}", Name = "GetClassById")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetClassesByIdQuery { Id = id };
        var classItem = await mediator.Send(query);
        if (classItem == null) return NotFound();
        return Ok(classItem);
    }

    // --- COMMAND: Create Class --- 📝

    /// <summary>
    /// Handles the creation of a new Class resource.
    /// </summary>
    /// <param name="command">The command containing data needed to create the Class.</param>
    /// <returns>
    /// A 201 Created response with the Location header for the new resource,
    /// or a 400 Bad Request if the creation failed due to validation or business logic errors.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClassesCommand command)
    {
        // Send the creation command to the mediator (CQRS handler) for processing.
        var serviceResponse = await mediator.Send(command);

        // Check if the business operation executed successfully.
        if (serviceResponse.Succeeded)
        {
            if (serviceResponse.Data is CreatedClassesDto createdClass)
            {
                // Successful creation: Return 201 Created with the Location header.
                return CreatedAtAction(
                    nameof(Get),
                    new { Id = createdClass.ClassId },
                    serviceResponse
                );
            }
            else
            {
                return StatusCode((int)HttpStatusCode.Created, serviceResponse);
            }
        }
        else
        {
            // Failure: Return 400 Bad Request.
            return BadRequest(serviceResponse);
        }
    }

    // --- COMMAND: Update Class --- 🔄

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClassesCommand command)
    {
        // 1. Ensure the ID in the route matches the ID in the command body
        // The DTO/record for UpdateClassesCommand should likely also contain the ID.
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
            // Best practice: Use a custom exception/result from the handler 
            // instead of string matching, but matching is kept for consistency.
            if (serviceResponse.Message?.Contains("not found", System.StringComparison.OrdinalIgnoreCase) == true)
            {
                // Return 404 Not Found if the record doesn't exist.
                return NotFound(serviceResponse);
            }

            // Consistent: Return 400 Bad Request for validation or other business rule failures.
            return BadRequest(serviceResponse);
        }
    }


    // --- COMMAND: Delete Class --- 🗑️

    [HttpDelete("{id:guid}")]
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
            // Best practice: Use a custom exception/result from the handler
            if (serviceResponse.Message?.Contains("not found", System.StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(serviceResponse);
            }

            // Return 400 Bad Request for validation errors or unexpected failures
            return BadRequest(serviceResponse);
        }
    }
}