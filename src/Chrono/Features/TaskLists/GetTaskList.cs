using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.TaskLists;

public record GetTaskList(int Id);

[Authorize]
[HttpGet("api/tasklists/{id:int}")]
[Tags("Tasklists")]
public class GetTaskListEndpoint(IApplicationDbContext context, ICurrentUserService currentUserService)
    : Endpoint<GetTaskList, TaskListDto>
{
    public override async Task HandleAsync(GetTaskList request, CancellationToken cancellationToken)
    {
        var taskList = await context.TaskLists
            .Include(x => x.Tasks)
            .ThenInclude(x => x.Categories)
            .ThenInclude(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (taskList == null)
        {
            throw new NotFoundException($"Task list \"{request.Id}\" not found.");
        }

        if (!taskList.IsPermitted(currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        await SendOkAsync(TaskListDto.FromEntity(taskList), cancellationToken);
    }
}
