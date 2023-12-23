using Chrono.Entities;
using Chrono.Shared.Api;
using Chrono.Shared.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.Features.Notes;

public record CreateNote(string Title, string Text) : IRequest<int>;

public class CreateNoteValidator : AbstractValidator<CreateNote>
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

public class CreateNoteHandler(IApplicationDbContext context) : IRequestHandler<CreateNote, int>
{
    public async Task<int> Handle(CreateNote request, CancellationToken cancellationToken)
    {
        var entity = new Note { Title = request.Title, Text = request.Text };
        context.Notes.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

[Authorize]
[Route("api/notes")]
[Tags("Notes")]
public class CreateNoteController : ApiControllerBase
{
    [HttpPost]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Create(CreateNote command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtRoute("GetNote", new { id = result }, JSendResponseBuilder.Success(result));
    }
}
