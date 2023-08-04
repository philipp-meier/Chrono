using Chrono.Application.Common.Extensions;
using Chrono.Application.Common.Interfaces;
using Chrono.Application.Entities;
using MediatR;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Application.Features.TaskLists;

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
        return new TaskListBriefDto { Id = task.Id, Title = task.Title };
    }
}
