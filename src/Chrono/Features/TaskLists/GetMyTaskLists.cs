using Chrono.Common.Api;
using Chrono.Common.Extensions;
using Chrono.Common.Interfaces;
using Chrono.Common.Services;
using Chrono.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Features.TaskLists;

public record GetMyTaskLists : IRequest<TaskListBriefDto[]>;

public class GetMyTaskListsHandler : IRequestHandler<GetMyTaskLists, TaskListBriefDto[]>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMyTaskListsHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public Task<TaskListBriefDto[]> Handle(GetMyTaskLists request, CancellationToken cancellationToken)
    {
        var result = _context.TaskLists
            .OrderBy(x => x.Title)
            .AsEnumerable()
            .Where(x => x.IsPermitted(_currentUserService.UserId))
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
        return new TaskListBriefDto
        {
            Id = task.Id, Title = task.Title
        };
    }
}

[Authorize] [Route("api/tasklists")] [Tags("Tasklists")]
public class GetMyTaskListsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await Mediator.Send(new GetMyTaskLists());
        return Ok(JSendResponseBuilder.Success(result));
    }
}
