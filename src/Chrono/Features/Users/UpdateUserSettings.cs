using Chrono.Shared.Api;
using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using Chrono.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Features.Users;

public record UpdateUserSettings(int? DefaultTaskListId) : IRequest;

public class UpdateUserSettingsHandler : IRequestHandler<UpdateUserSettings>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserSettingsHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateUserSettings request, CancellationToken cancellationToken)
    {
        var currentUser = await _context.Users
            .Include(x => x.UserSettings)
            .SingleOrDefaultAsync(x => x.UserId == _currentUserService.UserId, cancellationToken);

        if (currentUser == null)
        {
            throw new NotFoundException($"User \"{_currentUserService.UserId}\" not found.");
        }

        var userSettings = currentUser.UserSettings;
        if (userSettings == null)
        {
            userSettings = new UserSettings
            {
                User = currentUser, UserId = currentUser.Id
            };
            _context.UserSettings.Add(userSettings);
        }

        if (request.DefaultTaskListId.HasValue)
        {
            var defaultTaskListId = request.DefaultTaskListId.Value;
            var taskList = await _context.TaskLists
                .SingleOrDefaultAsync(x => x.Id == defaultTaskListId, cancellationToken);

            if (taskList == null)
            {
                throw new NotFoundException($"Task list \"{defaultTaskListId}\" not found.");
            }

            if (!taskList.IsPermitted(_currentUserService.UserId))
            {
                throw new ForbiddenAccessException();
            }

            userSettings.DefaultTaskList = taskList;
        }
        else
        {
            userSettings.DefaultTaskList = null;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

[Authorize] [Route("api/user")] [Tags("User")]
public class UpdateUserSettingsController : ApiControllerBase
{
    [HttpPut("settings")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Update(UpdateUserSettings command)
    {
        await Mediator.Send(command);
        return Ok(JSendResponseBuilder.Success<string>(null));
    }
}
