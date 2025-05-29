using Chrono.Entities;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Features.TaskLists;

public record GetMyTaskLists;

[Authorize]
[HttpGet("api/tasklists")]
[Tags("Tasklists")]
public class GetMyTaskListsEndpoint(IApplicationDbContext context, ICurrentUserService currentUserService)
    : EndpointWithoutRequest<TaskListBriefDto[]>
{
    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var result = context.TaskLists
            .OrderBy(x => x.Title)
            .AsEnumerable()
            .Where(x => x.IsPermitted(currentUserService.UserId))
            .Select(TaskListBriefDto.FromEntity)
            .ToArray();

        await SendOkAsync(result, cancellationToken);
    }
}

public class TaskListBriefDto
{
    public int Id { get; init; }
    public string Title { get; init; }

    public static TaskListBriefDto FromEntity(TaskList task)
    {
        return new TaskListBriefDto { Id = task.Id, Title = task.Title };
    }
}
