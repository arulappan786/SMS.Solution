using MediatR;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.CQRS.Core.Students.Commands;
using SMS.Application.CQRS.Core.Students.Queries;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Core;
using System.Net;

[ApiController]
[Route("api/students")] // Use a convention for the route
public class StudentsController(IMediator mediator) : ControllerBase
{
    // --- QUERY: Get All Students ---

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResultDto<StudentDto>), (int)HttpStatusCode.OK)]
    // ASP.NET Core automatically binds query string parameters to the GetAllStudentsQuery object
    public async Task<IActionResult> GetAll([FromQuery] GetAllStudentsQuery query)
    {
        var dto = await mediator.Send(query);

        // Returns 200 OK with the full pagination metadata
        return Ok(dto);
    }

    // --- QUERY: Get Student by ID ---

    [HttpGet("{id:guid}")] // Enforce GUID type constraint on the route
    [ProducesResponseType(typeof(StudentDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetStudentByIdQuery { StudentId = id };

        var dto = await mediator.Send(query);

        // Check if the handler returned null (resource not found)
        if (dto == null)
        {
            return NotFound($"Student with ID {id} not found."); // Returns 404 Not Found
        }

        return Ok(dto); // Returns 200 OK
    }

    // --- COMMAND: Create Student ---

    [HttpPost]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)] // Returns 201
    [ProducesResponseType((int)HttpStatusCode.BadRequest)] // For validation errors
    public async Task<IActionResult> Create([FromBody] CreateStudentCommand command)
    {
        // Mediator sends the command to the handler. 
        // We assume the handler returns the Guid of the newly created student.
        var newStudentId = await mediator.Send(command);

        // Returns 201 Created and provides the URI for the new resource.
        // Route name 'Get' must match the name of the Get method above.
        return CreatedAtAction(nameof(Get), new { id = newStudentId }, newStudentId);
    }
}