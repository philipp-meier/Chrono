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

public record GetNote(int Id) : IRequest<NoteDto>;

public class GetNoteHandler : IRequestHandler<GetNote, NoteDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetNoteHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<NoteDto> Handle(GetNote request, CancellationToken cancellationToken)
    {
        var note = await _context.Notes
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (note == null)
        {
            throw new NotFoundException($"Note \"{request.Id}\" not found.");
        }

        if (!note.IsPermitted(_currentUserService.UserId))
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

[Authorize] [Route("api/notes")] [Tags("Notes")]
public class GetNoteController : ApiControllerBase
{
    [HttpGet("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NoteDto>> Get(int id)
    {
        return await Mediator.Send(new GetNote(id));
    }
}
