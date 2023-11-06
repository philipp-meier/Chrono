using Chrono.Common.Api;
using Chrono.Common.Exceptions;
using Chrono.Common.Extensions;
using Chrono.Common.Interfaces;
using Chrono.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Notes;

public record DeleteNote(int Id) : IRequest;

public class DeleteNoteHandler : IRequestHandler<DeleteNote>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteNoteHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteNote request, CancellationToken cancellationToken)
    {
        var entity = await _context.Notes
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException($"Note \"{request.Id}\" not found.");
        }

        if (!entity.IsPermitted(_currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        _context.Notes.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

[Authorize] [Route("api/notes")] [Tags("Notes")]
public class DeleteNoteController : ApiControllerBase
{
    [HttpDelete("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteNote(id));
        return Ok(JSendResponseBuilder.Success<string>(null));
    }
}
