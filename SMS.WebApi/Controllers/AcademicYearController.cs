using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.Constants;
using SMS.Application.CQRS.Accademic.AcademicYears.Commands.Create;
using SMS.Application.CQRS.Accademic.AcademicYears.Commands.Delete;
using SMS.Application.CQRS.Accademic.AcademicYears.Commands.Update;
using SMS.Application.CQRS.Accademic.AcademicYears.Queries.GetAll;
using SMS.Application.CQRS.Accademic.AcademicYears.Queries.GetById;
using SMS.Application.DTOs.Accademic.AcademicYears;
using SMS.Application.DTOs.Service;
using System.Net;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.Admin)]
public class AcademicYearController(IMediator mediator) : ControllerBase
{
    // --- QUERY: Get All AcademicYears --- 📚

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllAcademicYearsQuery query)
    {
        var academicYears = await mediator.Send(query);
        return Ok(academicYears);
    }

    // --- QUERY: Get AcademicYear by ID ---

    [HttpGet("{id:guid}", Name = "GetAcademicYearById")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetAcademicYearByIdQuery { Id = id };
        var academicYear = await mediator.Send(query);
        if (academicYear == null) return NotFound();
        return Ok(academicYear);
    }

    // --- COMMAND: Create AcademicYear --- ✨

    /// <summary>
    /// Handles the creation of a new Academic Year resource.
    /// </summary>
    /// <param name="command">The command containing data needed to create the Academic Year.</param>
    /// <returns>A 201 Created response with the Location header for the new resource.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAcademicYearCommand command)
    {
        var serviceResponse = await mediator.Send(command);

        if (serviceResponse.Succeeded)
        {
            if (serviceResponse.Data is CreatedAcademicYearDto createdYear)
            {
                return CreatedAtAction(
                    nameof(Get),
                    new { Id = createdYear.AcademicYearId },
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
            return BadRequest(serviceResponse);
        }
    }

    // --- COMMAND: Update AcademicYear --- 🔄

    [HttpPut("{id:guid}")] // 👈 Added :guid constraint for safety
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAcademicYearCommand command)
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
            if (serviceResponse.Message?.Contains("not found", System.StringComparison.OrdinalIgnoreCase) == true)
            {
                // Return 404 Not Found if the record doesn't exist.
                return NotFound(serviceResponse);
            }

            // Consistent: Return 400 Bad Request for validation or other business rule failures.
            return BadRequest(serviceResponse);
        }
    }

    // --- COMMAND: Delete AcademicYear --- 🗑️

    [HttpDelete("{id:guid}")] // 👈 Added :guid constraint
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteAcademicYearCommand { Id = id };

        var serviceResponse = await mediator.Send(command);

        if (serviceResponse.Succeeded)
        {
            // Standard REST practice: 204 No Content on successful deletion
            return NoContent();
        }
        else
        {
            // Check for NotFound message from the handler
            if (serviceResponse.Message?.Contains("not found", System.StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(serviceResponse);
            }

            // Return 400 Bad Request for validation errors or unexpected failures
            return BadRequest(serviceResponse);
        }
    }
}