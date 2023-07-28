using Chrono.Application.TaskLists.Commands.CreateTaskList;
using Chrono.Application.TaskLists.Commands.DeleteTaskList;
using Chrono.Application.TaskLists.Queries.GetTaskList;
using Chrono.Application.TaskLists.Queries.GetTaskListOptions;
using Chrono.Application.TaskLists.Queries.GetTaskLists;
using Chrono.Application.Tasks.Commands.UpdateTaskList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.WebUI.Controllers;

[Authorize]
public class TaskListsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TaskListBriefDto[]>> Get()
    {
        return await Mediator.Send(new GetTaskListsQuery());
    }

    [HttpGet("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskListDto>> Get(int id)
    {
        return await Mediator.Send(new GetTaskListQuery(id));
    }

    [HttpGet("{id:int}/options")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskListOptionsDto>> GetOptions(int id)
    {
        return await Mediator.Send(new GetTaskListOptionsQuery(id));
    }

    [HttpPost]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<int>> Create(CreateTaskListCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Update(int id, UpdateTaskListCommand command)
    {
        if (id != command.TaskListId)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteTaskListCommand(id));
        return NoContent();
    }
}
