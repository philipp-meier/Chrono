using MediatR;
using Microsoft.EntityFrameworkCore;
using Chrono.Application.Common.Security;
using Chrono.Application.Common.Interfaces;
using Chrono.Application.Common.Exceptions;

namespace Chrono.Application.TaskLists.Queries.GetTaskList;

public record GetTaskListQuery(int ListId) : IRequest<TaskListDto>;

public class GetTaskListQueryHandler : IRequestHandler<GetTaskListQuery, TaskListDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTaskListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TaskListDto> Handle(GetTaskListQuery request, CancellationToken cancellationToken)
    {
        var taskList = await _context.TaskLists
            .Include(x => x.Tasks)
                .ThenInclude(x => x.Categories)
                .ThenInclude(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == request.ListId, cancellationToken);

        if (taskList == null)
            throw new NotFoundException($"Task list \"{request.ListId}\" not found.");

        if (!taskList.IsPermitted(_currentUserService.UserId))
            throw new ForbiddenAccessException();

        return TaskListDto.FromEntity(taskList);
    }
}
