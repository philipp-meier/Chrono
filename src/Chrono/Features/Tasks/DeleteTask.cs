using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Tasks;

public record DeleteTask(int Id);

[Authorize]
[HttpDelete("api/tasks/{id:int}")]
[Tags("Tasks")]
public class DeleteTaskEndpoint(IApplicationDbContext context, ICurrentUserService currentUserService)
    : Endpoint<DeleteTask>
{
    public override async Task HandleAsync(DeleteTask request, CancellationToken cancellationToken)
    {
        var entity = await context.Tasks
            .Include(x => x.List)
            .ThenInclude(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException($"Task item \"{request.Id}\" not found.");
        }

        if (!entity.IsPermitted(currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        if (entity.Done)
        {
            throw new InvalidOperationException("Task is already done.");
        }

        var taskList = entity.List;
        taskList.Tasks.Remove(entity);

        context.Tasks.Remove(entity);

        await context.SaveChangesAsync(cancellationToken);
        await SendOkAsync(cancellationToken);
    }
}
