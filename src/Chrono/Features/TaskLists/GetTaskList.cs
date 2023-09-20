using Chrono.Common.Api;
using Chrono.Common.Exceptions;
using Chrono.Common.Interfaces;
using Chrono.Entities;
using Chrono.Entities.Common;
using Chrono.Features.Tasks;
using Chrono.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.TaskLists;

[Authorize] [Route("api/tasklists")]
public class GetTaskListController : ApiControllerBase
{
    [HttpGet("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskListDto>> Get(int id)
    {
        return await Mediator.Send(new GetTaskList(id));
    }
}

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
