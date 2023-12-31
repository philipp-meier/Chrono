using Chrono.Shared.Api;
using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.TaskLists;

public record GetTaskList(int ListId) : IRequest<TaskListDto>;

public class GetTaskListHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    : IRequestHandler<GetTaskList, TaskListDto>
{
    public async Task<TaskListDto> Handle(GetTaskList request, CancellationToken cancellationToken)
    {
        var taskList = await context.TaskLists
            .Include(x => x.Tasks)
            .ThenInclude(x => x.Categories)
            .ThenInclude(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == request.ListId, cancellationToken);

        if (taskList == null)
        {
            throw new NotFoundException($"Task list \"{request.ListId}\" not found.");
        }

        if (!taskList.IsPermitted(currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        return TaskListDto.FromEntity(taskList);
    }
}

[Authorize]
[Route("api/tasklists")]
[Tags("Tasklists")]
public class GetTaskListController : ApiControllerBase
{
    [HttpGet("{id:int}", Name = "GetTaskList")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var result = await Mediator.Send(new GetTaskList(id));
        return Ok(JSendResponseBuilder.Success(result));
    }
}
