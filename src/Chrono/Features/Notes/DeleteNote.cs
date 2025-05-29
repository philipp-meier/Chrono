using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Notes;

public record DeleteNote(int Id);

[Authorize]
[HttpDelete("api/notes/{id:int}")]
[Tags("Notes")]
public class DeleteNoteEndpoint(IApplicationDbContext context, ICurrentUserService currentUserService)
    : Endpoint<DeleteNote, EmptyResponse>
{
    public override async Task HandleAsync(DeleteNote request, CancellationToken ct)
    {
        var entity = await context.Notes
            .SingleOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity == null)
        {
            throw new NotFoundException($"Note \"{request.Id}\" not found.");
        }

        if (!entity.IsPermitted(currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        context.Notes.Remove(entity);

        await context.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}
