using Microsoft.AspNetCore.Mvc;
using Chrono.Application.Common.Dtos;
using Microsoft.AspNetCore.Authorization;
using Chrono.Application.Tasks.Queries.GetTask;
using Chrono.Application.Tasks.Commands.CreateTask;
using Chrono.Application.Tasks.Commands.DeleteTask;
using Chrono.Application.Tasks.Commands.UpdateTask;

namespace Chrono.WebUI.Controllers;

[Authorize]
public class TasksController : ApiControllerBase
{
    [HttpGet("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> Get(int id)
        => await Mediator.Send(new GetTaskQuery(id));

    [HttpPost]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<int>> Create(CreateTaskCommand command)
        => await Mediator.Send(command);

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Update(int id, UpdateTaskCommand command)
    {
        if (id != command.Id)
            return BadRequest();

        await Mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteTaskCommand(id));
        return NoContent();
    }
}
