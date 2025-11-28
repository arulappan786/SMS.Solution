using MediatR;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.CQRS.Core.Students.Commands;
using SMS.Application.CQRS.Core.Students.Queries;
using SMS.Application.DTOs.Common;
using SMS.Application.DTOs.Core;
using System.Net;

[ApiController]
[Route("api/students")]
public class StudentsController(IMediator mediator) : ControllerBase
{
    // --- QUERY: Get All Students ---

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResultDto<StudentDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllStudentsQuery query)
    {
        var students = await mediator.Send(query);
        if (students == null) return NotFound($"No student record found");
        else return Ok(students);
    }

    // --- QUERY: Get Student by ID ---

    [HttpGet("{id:guid}")] // Enforce GUID type constraint on the route
    [ProducesResponseType(typeof(StudentDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetStudentByIdQuery { StudentId = id };
        var student = await mediator.Send(query);
        if (student == null) return NotFound($"Student with ID {id} not found.");
        else return Ok(student);
    }

    // --- COMMAND: Create Student ---

    [HttpPost]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)] // Returns 201
    [ProducesResponseType((int)HttpStatusCode.BadRequest)] // For validation errors
    public async Task<IActionResult> Create([FromBody] CreateStudentCommand command)
    {
        var serviceResponse = await mediator.Send(command);
        if (serviceResponse.Success) return StatusCode((int)HttpStatusCode.Created);
        else return BadRequest(serviceResponse);
    }
}