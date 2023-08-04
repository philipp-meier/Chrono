using Chrono.Application.Common.Dtos;
using Chrono.Application.Common.Exceptions;
using Chrono.Application.Common.Extensions;
using Chrono.Application.Common.Interfaces;
using Chrono.Application.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Application.Features.TaskLists;

public record GetTaskList(int ListId) : IRequest<TaskListDto>;

public class GetTaskListHandler : IRequestHandler<GetTaskList, TaskListDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTaskListHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TaskListDto> Handle(GetTaskList request, CancellationToken cancellationToken)
    {
        var taskList = await _context.TaskLists
            .Include(x => x.Tasks)
            .ThenInclude(x => x.Categories)
            .ThenInclude(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == request.ListId, cancellationToken);

        if (taskList == null)
        {
            throw new NotFoundException($"Task list \"{request.ListId}\" not found.");
        }

        if (!taskList.IsPermitted(_currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        return TaskListDto.FromEntity(taskList);
    }
}

public class TaskListDto
{
    public int Id { get; init; }
    public string Title { get; init; }
    public IReadOnlyCollection<TaskDto> Tasks { get; init; }

    public static TaskListDto FromEntity(TaskList taskList)
    {
        return new TaskListDto
        {
            Id = taskList.Id, Title = taskList.Title, Tasks = taskList.Tasks.Select(TaskDto.FromEntity).ToArray()
        };
    }
}
