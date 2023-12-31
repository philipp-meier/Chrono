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

public record GetNote(int Id) : IRequest<NoteDto>;

public class GetNoteHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    : IRequestHandler<GetNote, NoteDto>
{
    public async Task<NoteDto> Handle(GetNote request, CancellationToken cancellationToken)
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

        return new NoteDto
        {
            Id = note.Id,
            Title = note.Title,
            Text = note.Text,
            LastModified = DateTime.SpecifyKind(note.LastModified, DateTimeKind.Utc),
            LastModifiedBy = note.LastModifiedBy.Name
        };
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

[Authorize]
[Route("api/notes")]
[Tags("Notes")]
public class GetNoteController : ApiControllerBase
{
    [HttpGet("{id:int}", Name = "GetNote")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var result = await Mediator.Send(new GetNote(id));
        return Ok(JSendResponseBuilder.Success(result));
    }
}
