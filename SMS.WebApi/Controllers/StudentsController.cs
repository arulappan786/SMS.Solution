using MediatR;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.CQRS.Core.Students.Commands.CreateStudent;
using SMS.Application.CQRS.Core.Students.Queries.GetStudentList;
using SMS.Application.CQRS.Core.Students.Queries.GetStudentById;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Service;
using System.Net;
using SMS.Application.DTOs.Core.Students;

[Route("api/[controller]")]
[ApiController]
public class StudentsController(IMediator mediator) : ControllerBase
{
    // --- QUERY: Get All Students ---

    [HttpGet]
    // Assume handler returns PaginatedResultDto (empty list if none found, never null)
    [ProducesResponseType(typeof(PaginatedResultDto<StudentDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllStudentsQuery query)
    {
        var students = await mediator.Send(query);
        // Clean: Always returns 200 OK, with an empty list if no records exist.
        return Ok(students);
    }

    // --- QUERY: Get Student by ID ---

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StudentDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)] // Best handled by global filter
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetStudentByIdQuery { StudentId = id };
        var student = await mediator.Send(query);

        // If global exception handling is NOT used, keep this check:
        if (student == null) return NotFound();

        return Ok(student);
    }

    // --- COMMAND: Create Student ---

    [HttpPost]
    [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ServiceResponse), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateStudentCommand command)
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