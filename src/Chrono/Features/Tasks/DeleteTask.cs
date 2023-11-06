using Chrono.Common.Api;
using Chrono.Common.Exceptions;
using Chrono.Common.Extensions;
using Chrono.Common.Interfaces;
using Chrono.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Tasks;

public record DeleteTask(int Id) : IRequest;

public class DeleteTaskHandler : IRequestHandler<DeleteTask>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteTaskHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteTask request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tasks
            .Include(x => x.List)
            .ThenInclude(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException($"Task item \"{request.Id}\" not found.");
        }

        if (!entity.IsPermitted(_currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        if (entity.Done)
        {
            throw new InvalidOperationException("Task is already done.");
        }

        var taskList = entity.List;
        taskList.Tasks.Remove(entity);

        _context.Tasks.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

[Authorize] [Route("api/tasks")] [Tags("Tasks")]
public class DeleteTaskController : ApiControllerBase
{
    [HttpDelete("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteTask(id));
        return Ok(JSendResponseBuilder.Success<string>(null));
    }
}
