using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Tasks;

public record GetTask(int Id);

[Authorize]
[HttpGet("api/tasks/{id:int}")]
[Tags("Tasks")]
public class GetTaskEndpoint(IApplicationDbContext context, ICurrentUserService currentUserService)
    : Endpoint<GetTask, TaskDto>
{
    public override async Task HandleAsync(GetTask request, CancellationToken cancellationToken)
    {
        var task = await context.Tasks
            .Include(x => x.Categories)
            .ThenInclude(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (task == null)
        {
            throw new NotFoundException($"Task \"{request.Id}\" not found.");
        }

        if (!task.IsPermitted(currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        await SendOkAsync(TaskDto.FromEntity(task), cancellationToken);
    }
}
