using Chrono.Common.Api;
using Chrono.Common.Exceptions;
using Chrono.Common.Interfaces;
using Chrono.Entities;
using Chrono.Features.Audit;
using Chrono.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.TaskLists;

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
            RequireBusinessValue = taskListOptions?.RequireBusinessValue ?? true, RequireDescription = taskListOptions?.RequireDescription ?? true
        };
    }
}

[Authorize] [Route("api/tasklists")]
public class GetTaskListOptionsController : ApiControllerBase
{
    [HttpGet("{id:int}/options")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskListOptionsDto>> GetOptions(int id)
    {
        return await Mediator.Send(new GetTaskListOptions(id));
    }
}
