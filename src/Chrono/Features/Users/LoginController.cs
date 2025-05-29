using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.Features.Users;

[Authorize]
[ApiExplorerSettings(IgnoreApi = true)]
[FastEndpoints.HttpGet("api/login")]
public class LoginEndpoint : EndpointWithoutRequest
{
    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var sign = Query<string>("sign") ?? "in";

        if (sign == "out")
        {
            await CookieAuth.SignOutAsync();
        }

        await SendRedirectAsync("/");
    }
}
