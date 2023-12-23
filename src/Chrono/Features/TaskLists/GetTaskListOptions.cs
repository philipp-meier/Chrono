using Chrono.Entities;
using Chrono.Shared.Api;
using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.TaskLists;

public record GetTaskListOptions(int ListId) : IRequest<TaskListOptionsDto>;

public class GetTaskListOptionsHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    : IRequestHandler<GetTaskListOptions, TaskListOptionsDto>
{
    public async Task<TaskListOptionsDto> Handle(GetTaskListOptions request, CancellationToken cancellationToken)
    {
        var taskList = await context.TaskLists
            .SingleOrDefaultAsync(x => x.Id == request.ListId, cancellationToken);

        if (taskList == null)
        {
            throw new NotFoundException($"Task list \"{request.ListId}\" not found.");
        }

        if (!taskList.IsPermitted(currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        return TaskListOptionsDto.FromEntity(taskList.Options);
    }
}

public class TaskListOptionsDto
{
    public bool RequireBusinessValue { get; set; }
    public bool RequireDescription { get; set; }

    public static TaskListOptionsDto FromEntity(TaskListOptions taskListOptions)
    {
        return new TaskListOptionsDto
        {
            RequireBusinessValue = taskListOptions?.RequireBusinessValue ?? true,
            RequireDescription = taskListOptions?.RequireDescription ?? true
        };
    }
}

[Authorize]
[Route("api/tasklists")]
[Tags("Tasklists")]
public class GetTaskListOptionsController : ApiControllerBase
{
    [HttpGet("{id:int}/options")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOptions(int id)
    {
        var result = await Mediator.Send(new GetTaskListOptions(id));
        return Ok(JSendResponseBuilder.Success(result));
    }
}
