using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Chrono.Application.UserInfo.Queries.GetUserInfo;

namespace Chrono.WebUI.Controllers;

public class UserController : ApiControllerBase
{
    [HttpGet, AllowAnonymous]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<UserInfoDto>> Get()
        => await Mediator.Send(new GetUserInfoQuery());
}
