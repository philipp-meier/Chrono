using Chrono.Shared.Api;
using Chrono.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.Features.Users;

public record GetUserInfo : IRequest<UserInfoDto>;

public class GetUserInfoHandler(ICurrentUserService currentUserService) : IRequestHandler<GetUserInfo, UserInfoDto>
{
    public Task<UserInfoDto> Handle(GetUserInfo request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new UserInfoDto
        {
            Username = currentUserService.UserName, IsAuthenticated = currentUserService.IsAuthenticated
        });
    }
}

public class UserInfoDto
{
    public string Username { get; init; }
    public bool IsAuthenticated { get; init; }
}

[Authorize]
[Route("api/user")]
[Tags("User")]
public class GetUserInfoController : ApiControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Get()
    {
        var result = await Mediator.Send(new GetUserInfo());
        return Ok(JSendResponseBuilder.Success(result));
    }
}
