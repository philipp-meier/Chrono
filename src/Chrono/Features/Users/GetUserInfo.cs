using Chrono.Shared.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace Chrono.Features.Users;

[AllowAnonymous]
[HttpGet("api/user")]
[Tags("User")]
public class GetUserInfoEndpoint(ICurrentUserService currentUserService) : EndpointWithoutRequest<UserInfoDto>
{
    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var response = new UserInfoDto
        {
            Username = currentUserService.UserName, IsAuthenticated = currentUserService.IsAuthenticated
        };

        await SendOkAsync(response, cancellationToken);
    }
}

public class UserInfoDto
{
    public string Username { get; init; }
    public bool IsAuthenticated { get; init; }
}
