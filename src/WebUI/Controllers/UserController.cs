using Chrono.Application.Features.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.WebUI.Controllers;

[Authorize]
public class UserController : ApiControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<UserInfoDto>> Get()
    {
        return await Mediator.Send(new GetUserInfo());
    }

    [HttpGet("settings")]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<UserSettingsDto>> GetSettings()
    {
        return await Mediator.Send(new GetUserSettings());
    }


    [HttpPut("settings")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Update(UpdateUserSettings command)
    {
        await Mediator.Send(command);
        return NoContent();
    }
}
