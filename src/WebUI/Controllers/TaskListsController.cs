using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Chrono.Application.TaskLists.Queries.GetTaskList;
using Chrono.Application.TaskLists.Queries.GetTaskLists;
using Chrono.Application.TaskLists.Commands.CreateTaskList;
using Chrono.Application.TaskLists.Commands.DeleteTaskList;

namespace Chrono.WebUI.Controllers;

[Authorize]
public class TaskListsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TaskListBriefDto[]>> Get()
        => await Mediator.Send(new GetTaskListsQuery());

    [HttpGet("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskListDto>> Get(int id)
        => await Mediator.Send(new GetTaskListQuery(id));

    [HttpPost]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<int>> Create(CreateTaskListCommand command)
        => await Mediator.Send(command);

    [HttpDelete("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteTaskListCommand(id));
        return NoContent();
    }
}
