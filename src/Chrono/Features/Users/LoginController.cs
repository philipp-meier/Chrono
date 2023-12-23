using System.Web;
using Chrono.Shared.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.Features.Users;

[Authorize]
[ApiExplorerSettings(IgnoreApi = true)]
public class LoginController(IConfiguration config) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] string redirectUrl, [FromQuery] string sign = "in")
    {
        if (sign == "in")
        {
            return Redirect(redirectUrl);
        }

        var idToken = await HttpContext.GetTokenAsync("id_token");
        var idpHost = config["IdentityProvider:Authority"];

        SignOut("cookie", "oidc");

        // To ensure that all auth. cookies are being deleted, since ASP.NET Core uses the ChunkingCookieManager for cookie authentication by default.
        new ChunkingCookieManager().DeleteCookie(HttpContext, config["IdentityProvider:CookieName"]!,
            new CookieOptions());

        return Redirect(idpHost + "oidc/logout?id_token_hint=" + idToken + "&post_logout_redirect_uri=" +
                        HttpUtility.UrlEncode(redirectUrl));
    }
}
