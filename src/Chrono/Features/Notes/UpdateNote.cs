using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Notes;

public record UpdateNote
{
    public int Id { get; init; }
    public string Title { get; set; }
    public string Text { get; set; }
    public bool? IsPinned { get; set; }
}

public class UpdateNoteValidator : Validator<UpdateNote>
{
    public UpdateNoteValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty();

        RuleFor(v => v.Title)
            .MaximumLength(100)
            .NotEmpty();
    }
}

[Authorize]
[HttpPut("api/notes/{id:int}")]
[Tags("Notes")]
public class UpdateNoteEndpoint(IApplicationDbContext context, ICurrentUserService currentUserService)
    : Endpoint<UpdateNote>
{
    public override async Task HandleAsync(UpdateNote request, CancellationToken cancellationToken)
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

        if (note.Title != request.Title)
        {
            note.Title = request.Title;
        }

        if (request.Text != null && note.Text != request.Text)
        {
            note.Text = request.Text;
        }

        if (request.IsPinned.HasValue && note.IsPinned != request.IsPinned)
        {
            note.IsPinned = request.IsPinned.Value;
        }

        await context.SaveChangesAsync(cancellationToken);
        await SendOkAsync(cancellationToken);
    }
}
