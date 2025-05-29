using Chrono.Entities;
using Chrono.Shared.Interfaces;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Features.Notes;

public record CreateNote(string Title, string Text);

[Authorize]
[HttpPost("api/notes")]
[Tags("Notes")]
public class CreateNoteEndpoint(IApplicationDbContext context) : Endpoint<CreateNote, int>
{
    public override async Task HandleAsync(CreateNote request, CancellationToken ct)
    {
        var entity = new Note { Title = request.Title, Text = request.Text };
        context.Notes.Add(entity);

        await context.SaveChangesAsync(ct);

        await SendCreatedAtAsync<GetNoteEndpoint>(new { id = entity.Id }, entity.Id, cancellation: ct);
    }
}

public class CreateNoteValidator : Validator<CreateNote>
{
    public CreateNoteValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(100)
            .NotEmpty();

        RuleFor(v => v.Text)
            .NotEmpty();
    }
}
