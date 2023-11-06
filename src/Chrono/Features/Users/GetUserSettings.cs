using Chrono.Common.Api;
using Chrono.Common.Exceptions;
using Chrono.Common.Interfaces;
using Chrono.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Users;

public record GetUserSettings : IRequest<UserSettingsDto>;

public class GetUserSettingsHandler : IRequestHandler<GetUserSettings, UserSettingsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetUserSettingsHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<UserSettingsDto> Handle(GetUserSettings request, CancellationToken cancellationToken)
    {
        var currentUser = await _context.Users
            .Include(x => x.UserSettings)
            .SingleOrDefaultAsync(x => x.UserId == _currentUserService.UserId, cancellationToken);

        if (currentUser == null)
        {
            throw new NotFoundException($"User \"{_currentUserService.UserId}\" not found.");
        }

        return new UserSettingsDto
        {
            DefaultTaskListId = currentUser.UserSettings?.DefaultTaskList?.Id
        };
    }
}

public class UserSettingsDto
{
    public int? DefaultTaskListId { get; init; }
}

[Authorize] [Route("api/user")] [Tags("User")]
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
