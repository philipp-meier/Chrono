using System.Globalization;
using Chrono.Shared.Api;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.Features.Notes;

public record GetMyNotes : IRequest<GetMyNotesResponse>;

public class GetMyNotesHandler : IRequestHandler<GetMyNotes, GetMyNotesResponse>
{
    private const int MaxTextPreviewLength = 80;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly TextService _textService;

    public GetMyNotesHandler(IApplicationDbContext context, ICurrentUserService currentUserService, TextService textService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _textService = textService;
    }

    public Task<GetMyNotesResponse> Handle(GetMyNotes request, CancellationToken cancellationToken)
    {
        var result = _context.Notes
            .OrderByDescending(n => n.Created)
            .AsEnumerable()
            .Where(n => n.IsPermitted(_currentUserService.UserId))
            .Select(n => new NotePreview
            {
                Id = n.Id,
                Title = n.Title,
                Preview = _textService.Truncate(n.Text, MaxTextPreviewLength),
                // Persisted in UTC.
                Created = n.Created.ToLocalTime().ToString(CultureInfo.InvariantCulture)
            })
            .ToArray();

        return Task.FromResult(new GetMyNotesResponse
        {
            Notes = result
        });
    }
}

public class GetMyNotesResponse
{
    public NotePreview[] Notes { get; set; }
}

public class NotePreview
{
    public int Id { get; init; }
    public string Title { get; init; }
    public string Preview { get; set; }
    public string Created { get; set; }
}

[Authorize] [Route("api/notes")] [Tags("Notes")]
public class GetMyNotesController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMyNotes()
    {
        var result = await Mediator.Send(new GetMyNotes());
        return Ok(JSendResponseBuilder.Success(result));
    }
}
