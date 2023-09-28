using Chrono.Common.Api;
using Chrono.Common.Extensions;
using Chrono.Common.Interfaces;
using Chrono.Common.Services;
using Chrono.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Features.Notes;

public record GetMyNotes : IRequest<NoteBriefDto[]>;

public class GetMyNotesHandler : IRequestHandler<GetMyNotes, NoteBriefDto[]>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMyNotesHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public Task<NoteBriefDto[]> Handle(GetMyNotes request, CancellationToken cancellationToken)
    {
        var result = _context.Notes
            .OrderBy(x => x.LastModified)
            .AsEnumerable()
            .Where(x => x.IsPermitted(_currentUserService.UserId))
            .Select(NoteBriefDto.FromEntity)
            .ToArray();

        return Task.FromResult(result);
    }
}

public class NoteBriefDto
{
    public int Id { get; init; }
    public string Title { get; init; }
    public DateTime LastModified { get; set; }

    public static NoteBriefDto FromEntity(Note note)
    {
        return new NoteBriefDto
        {
            Id = note.Id, Title = note.Title, LastModified = note.LastModified
        };
    }
}

[Authorize] [Route("api/notes")] [Tags("Notes")]
public class GetMyNotesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<NoteBriefDto[]>> GetMyNotes()
    {
        return await Mediator.Send(new GetMyNotes());
    }
}
