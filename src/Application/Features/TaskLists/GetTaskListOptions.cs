using Chrono.Application.Common.Exceptions;
using Chrono.Application.Common.Extensions;
using Chrono.Application.Common.Interfaces;
using Chrono.Application.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Application.Features.TaskLists;

public record GetTaskListOptions(int ListId) : IRequest<TaskListOptionsDto>;

public class GetTaskListOptionsHandler : IRequestHandler<GetTaskListOptions, TaskListOptionsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTaskListOptionsHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TaskListOptionsDto> Handle(GetTaskListOptions request, CancellationToken cancellationToken)
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
