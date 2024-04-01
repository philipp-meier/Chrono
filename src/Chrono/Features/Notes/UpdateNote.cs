using Chrono.Shared.Api;
using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Notes;

public record UpdateNote : IRequest
{
    public int Id { get; init; }
    public string Title { get; set; }
    public string? Text { get; set; }
    public bool? IsPinned { get; set; }
}

public class UpdateNoteValidator : AbstractValidator<UpdateNote>
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

public class UpdateNoteHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    : IRequestHandler<UpdateNote>
{
    public async Task Handle(UpdateNote request, CancellationToken cancellationToken)
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
    }
}

[Authorize]
[Route("api/notes")]
[Tags("Notes")]
public class UpdateNoteController : ApiControllerBase
{
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Update(int id, UpdateNote command)
    {
        if (id != command.Id)
        {
            return BadRequest(JSendResponseBuilder.Fail<string>(null, "ID mismatch"));
        }

        await Mediator.Send(command);

        return Ok(JSendResponseBuilder.Success<string>(null));
    }
}
