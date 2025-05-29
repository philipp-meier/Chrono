using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Notes;

public record GetNote(int Id);

[Authorize]
[HttpGet("api/notes/{id:int}")]
[Tags("Notes")]
public class GetNoteEndpoint(IApplicationDbContext context, ICurrentUserService currentUserService)
    : Endpoint<GetNote, NoteDto>
{
    public override async Task HandleAsync(GetNote request, CancellationToken cancellationToken)
    {
        var note = await context.Notes
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (note == null)
        {
            throw new NotFoundException($"Note \"{request.Id}\" not found.");
        }

        if (!note.IsPermitted(currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        await SendOkAsync(
            new NoteDto
            {
                Id = note.Id,
                Title = note.Title,
                Text = note.Text,
                LastModified = DateTime.SpecifyKind(note.LastModified, DateTimeKind.Utc),
                LastModifiedBy = note.LastModifiedBy.Name
            }, cancellationToken);
    }
}

public class NoteDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public string LastModifiedBy { get; init; }
    public DateTime LastModified { get; init; }
}
