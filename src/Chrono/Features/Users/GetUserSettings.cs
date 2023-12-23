using Chrono.Shared.Api;
using Chrono.Shared.Exceptions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Users;

public record GetUserSettings : IRequest<UserSettingsDto>;

public class GetUserSettingsHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    : IRequestHandler<GetUserSettings, UserSettingsDto>
{
    public async Task<UserSettingsDto> Handle(GetUserSettings request, CancellationToken cancellationToken)
    {
        var currentUser = await context.Users
            .Include(x => x.UserSettings)
            .SingleOrDefaultAsync(x => x.UserId == currentUserService.UserId, cancellationToken);

        if (currentUser == null)
        {
            throw new NotFoundException($"User \"{currentUserService.UserId}\" not found.");
        }

        return new UserSettingsDto { DefaultTaskListId = currentUser.UserSettings?.DefaultTaskList?.Id };
    }
}

public class UserSettingsDto
{
    public int? DefaultTaskListId { get; init; }
}

[Authorize]
[Route("api/user")]
[Tags("User")]
public class GetUserSettingsController : ApiControllerBase
{
    [HttpGet("settings")]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> GetSettings()
    {
        var result = await Mediator.Send(new GetUserSettings());
        return Ok(JSendResponseBuilder.Success(result));
    }
}
