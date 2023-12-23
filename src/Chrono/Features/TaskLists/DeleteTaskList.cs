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

public record DeleteTaskList(int Id) : IRequest;

public class DeleteTaskListHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    : IRequestHandler<DeleteTaskList>
{
    public async Task Handle(DeleteTaskList request, CancellationToken cancellationToken)
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
    }
}

[Authorize]
[Route("api/tasklists")]
[Tags("Tasklists")]
public class DeleteTaskListsController : ApiControllerBase
{
    [HttpDelete("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteTaskList(id));
        return Ok(JSendResponseBuilder.Success<string>(null));
    }
}
