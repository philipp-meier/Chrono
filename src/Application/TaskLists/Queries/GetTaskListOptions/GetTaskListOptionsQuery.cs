using Chrono.Application.Common.Exceptions;
using Chrono.Application.Common.Interfaces;
using Chrono.Application.Common.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Application.TaskLists.Queries.GetTaskListOptions;

public record GetTaskListOptionsQuery(int ListId) : IRequest<TaskListOptionsDto>;

public class GetTaskListOptionsQueryHandler : IRequestHandler<GetTaskListOptionsQuery, TaskListOptionsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTaskListOptionsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TaskListOptionsDto> Handle(GetTaskListOptionsQuery request, CancellationToken cancellationToken)
    {
        var taskList = await _context.TaskLists
            .SingleOrDefaultAsync(x => x.Id == request.ListId, cancellationToken);

        if (taskList == null)
        {
            throw new NotFoundException($"Task list \"{request.ListId}\" not found.");
        }

        if (!taskList.IsPermitted(_currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        return TaskListOptionsDto.FromEntity(taskList.Options);
    }
}
