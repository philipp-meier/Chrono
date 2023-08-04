using Chrono.Application.Features.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.WebUI.Controllers;

public class UserController : ApiControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<UserInfoDto>> Get()
    {
        return await Mediator.Send(new GetUserInfo());
    }
}
