using Chrono.Shared.Api;
using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Notes;

public record DeleteNote(int Id) : IRequest;

public class DeleteNoteHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    : IRequestHandler<DeleteNote>
{
    public async Task Handle(DeleteNote request, CancellationToken cancellationToken)
    {
        var entity = await context.Notes
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException($"Note \"{request.Id}\" not found.");
        }

        if (!entity.IsPermitted(currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        context.Notes.Remove(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}

[Authorize]
[Route("api/notes")]
[Tags("Notes")]
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
