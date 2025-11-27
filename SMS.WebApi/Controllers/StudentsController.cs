using MediatR;
using Microsoft.AspNetCore.Mvc;
using SMS.Application.CQRS.Core.Students.Commands;
using SMS.Application.CQRS.Core.Students.Queries;

namespace SMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(CreateStudentCommand command)
        {
            return Ok(await mediator.Send(command));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var query = new GetStudentByIdQuery { StudentId = id };
            var dto = await mediator.Send(query);
            return Ok(dto);
        }
    }
}
