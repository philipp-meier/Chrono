using Chrono.Shared.Exceptions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Users;

[Authorize]
[HttpGet("api/user/settings")]
[Tags("User")]
public class GetUserSettingsEndpoint(IApplicationDbContext context, ICurrentUserService currentUserService)
    : EndpointWithoutRequest<UserSettingsDto>
{
    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var currentUser = await context.Users
            .Include(x => x.UserSettings)
            .SingleOrDefaultAsync(x => x.UserId == currentUserService.UserId, cancellationToken);

        if (currentUser == null)
        {
            throw new NotFoundException($"User \"{currentUserService.UserId}\" not found.");
        }

        await SendOkAsync(new UserSettingsDto { DefaultTaskListId = currentUser.UserSettings?.DefaultTaskList?.Id },
            cancellationToken);
    }
}

public class UserSettingsDto
{
    public int? DefaultTaskListId { get; init; }
}
