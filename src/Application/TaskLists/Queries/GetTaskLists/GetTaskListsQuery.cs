using MediatR;
using Chrono.Application.Common.Security;
using Chrono.Application.Common.Interfaces;

namespace Chrono.Application.TaskLists.Queries.GetTaskLists;

public record GetTaskListsQuery : IRequest<TaskListBriefDto[]>;

public class GetTaskListQueryHandler : IRequestHandler<GetTaskListsQuery, TaskListBriefDto[]>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTaskListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public Task<TaskListBriefDto[]> Handle(GetTaskListsQuery request, CancellationToken cancellationToken)
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
