using Chrono.Entities;
using Chrono.Shared.Api;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Features.TaskLists;

public record GetMyTaskLists : IRequest<TaskListBriefDto[]>;

public class GetMyTaskListsHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    : IRequestHandler<GetMyTaskLists, TaskListBriefDto[]>
{
    public Task<TaskListBriefDto[]> Handle(GetMyTaskLists request, CancellationToken cancellationToken)
    {
        var result = context.TaskLists
            .OrderBy(x => x.Title)
            .AsEnumerable()
            .Where(x => x.IsPermitted(currentUserService.UserId))
            .Select(TaskListBriefDto.FromEntity)
            .ToArray();

        return Task.FromResult(result);
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

[Authorize]
[Route("api/tasklists")]
[Tags("Tasklists")]
public class GetMyTaskListsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await Mediator.Send(new GetMyTaskLists());
        return Ok(JSendResponseBuilder.Success(result));
    }
}
