using Chrono.Entities;
using Chrono.Shared.Api;
using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Features.Users;

public record UpdateUserSettings(int? DefaultTaskListId) : IRequest;

public class UpdateUserSettingsHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    : IRequestHandler<UpdateUserSettings>
{
    public async Task Handle(UpdateUserSettings request, CancellationToken cancellationToken)
    {
        var currentUser = await context.Users
            .Include(x => x.UserSettings)
            .SingleOrDefaultAsync(x => x.UserId == currentUserService.UserId, cancellationToken);

        if (currentUser == null)
        {
            throw new NotFoundException($"User \"{currentUserService.UserId}\" not found.");
        }

        var userSettings = currentUser.UserSettings;
        if (userSettings == null)
        {
            userSettings = new UserSettings { User = currentUser, UserId = currentUser.Id };
            context.UserSettings.Add(userSettings);
        }

        if (request.DefaultTaskListId.HasValue)
        {
            var defaultTaskListId = request.DefaultTaskListId.Value;
            var taskList = await context.TaskLists
                .SingleOrDefaultAsync(x => x.Id == defaultTaskListId, cancellationToken);

            if (taskList == null)
            {
                throw new NotFoundException($"Task list \"{defaultTaskListId}\" not found.");
            }

            if (!taskList.IsPermitted(currentUserService.UserId))
            {
                throw new ForbiddenAccessException();
            }

            userSettings.DefaultTaskList = taskList;
        }
        else
        {
            userSettings.DefaultTaskList = null;
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}

[Authorize]
[Route("api/user")]
[Tags("User")]
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
