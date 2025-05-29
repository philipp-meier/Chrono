using Chrono.Entities;
using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Features.TaskLists;

public record GetTaskListOptions(int ListId);

[Authorize]
[HttpGet("api/tasklists/{id:int}/options")]
[Tags("Tasklists")]
public class GetTaskListOptionsHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    : Endpoint<GetTaskListOptions, TaskListOptionsDto>
{
    public override async Task HandleAsync(GetTaskListOptions request,
        CancellationToken cancellationToken)
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

        await SendOkAsync(TaskListOptionsDto.FromEntity(taskList.Options), cancellationToken);
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
