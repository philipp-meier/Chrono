using Chrono.Shared.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.Features.Users;

[Authorize]
[ApiExplorerSettings(IgnoreApi = true)]
public class LoginController : ApiControllerBase
{
    [HttpGet]
    public ActionResult Get([FromQuery] string redirectUrl, [FromQuery] string sign = "in")
    {
        if (sign == "in")
        {
            return Redirect(redirectUrl);
        }

        return SignOut(new AuthenticationProperties { RedirectUri = redirectUrl },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}
