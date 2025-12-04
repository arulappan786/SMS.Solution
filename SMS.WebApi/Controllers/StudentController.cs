using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.Constants;
using SMS.Application.CQRS.Core.Students.Commands.Create;
using SMS.Application.CQRS.Core.Students.Commands.Delete;
using SMS.Application.CQRS.Core.Students.Commands.Update;
using SMS.Application.CQRS.Core.Students.Queries.GetAll;
using SMS.Application.CQRS.Core.Students.Queries.GetById;
using SMS.Application.DTOs.Core.Students;
using SMS.Application.DTOs.Service;
using System.Net;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.Admin)]
public class StudentController(IMediator mediator) : ControllerBase
{
    // --- QUERY: Get All Students ---

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllStudentsQuery query)
    {
        var students = await mediator.Send(query);
        return Ok(students);
    }

    // --- QUERY: Get Student by ID ---

    [HttpGet("{id:guid}", Name = "GetStudentById")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetStudentByIdQuery { Id = id };
        var student = await mediator.Send(query);

        // If global exception handling is NOT used, keep this check:
        if (student == null) return NotFound();

        return Ok(student);
    }

    // --- COMMAND: Create Student ---

    /// <summary>
    /// Handles the creation of a new student resource.
    /// </summary>
    /// <param name="command">The command containing data needed to create the student.</param>
    /// <returns>
    /// A 201 Created response with the Location header for the new resource,
    /// or a 400 Bad Request if the creation failed due to validation or business logic errors.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentCommand command)
    {
        // Send the creation command to the mediator (CQRS handler) for processing.
        var serviceResponse = await mediator.Send(command);

        // Check if the business operation executed successfully.
        if (serviceResponse.Succeeded)
        {
            // Use pattern matching to safely cast the response data to the expected DTO record (CreatedStudentDto).
            if (serviceResponse.Data is CreatedStudentDto createdStudent)
            {
                // Successful creation: Return 201 Created with the Location header.
                // CreatedAtAction builds the full URI for the GET endpoint (named 'Get') 
                // using the newly created StudentId, adhering to REST best practices.
                return CreatedAtAction(
                    nameof(Get),
                    new { Id = createdStudent.StudentId }, // Assuming property is StudentId or Id
                    serviceResponse
                );
            }
            else
            {
                // Fallback: If data structure is unexpectedly missing or incorrect, 
                // return a simple 201 Created without the Location header (less RESTful).
                return StatusCode((int)HttpStatusCode.Created, serviceResponse);
            }
        }
        else
        {
            // Failure: Return 400 Bad Request, including the ServiceResponse 
            // which contains the specific error message(s).
            return BadRequest(serviceResponse);
        }
    }

    // --- COMMAND: Update Student ---

    [HttpPut("{id:guid}")] // 👈 Added :guid constraint for route safety
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentCommand command)
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
            if (serviceResponse.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                // Return 404 Not Found if the record doesn't exist.
                return NotFound(serviceResponse);
            }

            // Consistent: Return 400 Bad Request for validation or other business rule failures.
            return BadRequest(serviceResponse);
        }
    }


    // --- COMMAND: Delete Student ---

    [HttpDelete("{id:guid}")] // 👈 Added :guid constraint for route safety
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteStudentCommand { Id = id };

        var serviceResponse = await mediator.Send(command);

        if (serviceResponse.Succeeded)
        {
            // Standard REST practice: 204 No Content on successful deletion
            return NoContent();
        }
        else
        {
            // Check for NotFound message from the handler
            if (serviceResponse.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            {
                return NotFound(serviceResponse);
            }

            // Return 400 Bad Request for validation errors or unexpected failures
            return BadRequest(serviceResponse);
        }
    }
}