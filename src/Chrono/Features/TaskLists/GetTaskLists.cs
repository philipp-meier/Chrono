using Chrono.Common.Api;
using Chrono.Common.Interfaces;
using Chrono.Entities;
using Chrono.Entities.Common;
using Chrono.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Features.TaskLists;

[Authorize] [Route("api/tasklists")]
public class GetTaskListsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TaskListBriefDto[]>> Get()
    {
        return await Mediator.Send(new GetTaskLists());
    }
}

public record GetTaskLists : IRequest<TaskListBriefDto[]>;

public class GetTaskListsHandler : IRequestHandler<GetTaskLists, TaskListBriefDto[]>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTaskListsHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public Task<TaskListBriefDto[]> Handle(GetTaskLists request, CancellationToken cancellationToken)
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
