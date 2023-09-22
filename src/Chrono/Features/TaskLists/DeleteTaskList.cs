using Chrono.Common.Api;
using Chrono.Common.Exceptions;
using Chrono.Common.Extensions;
using Chrono.Common.Interfaces;
using Chrono.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.TaskLists;

public record DeleteTaskList(int Id) : IRequest;

public class DeleteTaskListHandler : IRequestHandler<DeleteTaskList>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteTaskListHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteTaskList request, CancellationToken cancellationToken)
    {
        var entity = await _context.TaskLists
            .Include(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException($"Task list \"{request.Id}\" not found.");
        }

        if (!entity.IsPermitted(_currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        _context.Tasks.RemoveRange(entity.Tasks);
        _context.TaskLists.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

[Authorize] [Route("api/tasklists")] [Tags("Tasklists")]
public class DeleteTaskListsController : ApiControllerBase
{
    [HttpDelete("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteTaskList(id));
        return NoContent();
    }
}
