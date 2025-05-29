using System.Globalization;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Chrono.Features.Notes;

[Authorize]
[HttpGet("api/notes")]
[Tags("Notes")]
public class GetMyNotesEndpoint(IApplicationDbContext context, ICurrentUserService currentUserService)
    : EndpointWithoutRequest<GetMyNotesResponse>
{
    private const int MaxTextPreviewLength = 80;

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = context.Notes
            .OrderByDescending(n => n.Created)
            .AsEnumerable()
            .Where(n => n.IsPermitted(currentUserService.UserId))
            .Select(n => new NotePreview
            {
                Id = n.Id,
                Title = n.Title,
                Preview = TextService.Truncate(n.Text, MaxTextPreviewLength),
                IsPinned = n.IsPinned,
                // Persisted in UTC.
                Created = n.Created.ToLocalTime().ToString(CultureInfo.InvariantCulture)
            })
            .ToArray();

        await SendOkAsync(new GetMyNotesResponse { Notes = result }, ct);
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
    public bool IsPinned { get; set; }
    public string Created { get; set; }
}
