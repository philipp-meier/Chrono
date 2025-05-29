using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.TaskLists;

public record DeleteTaskList(int Id);

[Authorize]
[HttpDelete("api/tasklists/{id:int}")]
[Tags("Tasklists")]
public class DeleteTaskListEndpoint(IApplicationDbContext context, ICurrentUserService currentUserService)
    : Endpoint<DeleteTaskList>
{
    public override async Task HandleAsync(DeleteTaskList request, CancellationToken cancellationToken)
    {
        var entity = await context.TaskLists
            .Include(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException($"Task list \"{request.Id}\" not found.");
        }

        if (!entity.IsPermitted(currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        context.Tasks.RemoveRange(entity.Tasks);
        context.TaskLists.Remove(entity);

        await context.SaveChangesAsync(cancellationToken);
        await SendOkAsync(cancellationToken);
    }
}
