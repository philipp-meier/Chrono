using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Chrono.WebUI.Controllers;

[Authorize, ApiExplorerSettings(IgnoreApi = true)]
public class LoginController : ApiControllerBase
{
    private readonly IConfiguration _configuration;

    public LoginController(IConfiguration config)
    {
        _configuration = config;
    }

    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] string redirectUrl, [FromQuery] string sign = "in")
    {
        if (sign == "in")
            return Redirect(redirectUrl);

        var idToken = await HttpContext.GetTokenAsync("id_token");
        var idpHost = _configuration["IdentityProvider:Authority"];

        SignOut("cookie", "oidc");

        // To ensure that all auth. cookies are being deleted, since ASP.NET Core uses the ChunkingCookieManager for cookie authentication by default.
        new ChunkingCookieManager().DeleteCookie(HttpContext, _configuration["IdentityProvider:CookieName"], new CookieOptions());

        return Redirect(idpHost + "oidc/logout?id_token_hint=" + idToken + "&post_logout_redirect_uri=" + HttpUtility.UrlEncode(redirectUrl));
    }
}
