using Chrono.Common.Api;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.Features.Users;

[Authorize] [Route("api/user")]
public class GetUserInfoController : ApiControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<UserInfoDto>> Get()
    {
        return await Mediator.Send(new GetUserInfo());
    }
}

public record GetUserInfo : IRequest<UserInfoDto>;

public class GetUserInfoHandler : IRequestHandler<GetUserInfo, UserInfoDto>
{
    private readonly ICurrentUserService _currentUserService;

    public GetUserInfoHandler(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<UserInfoDto> Handle(GetUserInfo request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(new UserInfoDto
        {
            Username = _currentUserService.UserName, IsAuthenticated = _currentUserService.IsAuthenticated
        });
    }
}

public class UserInfoDto
{
    public string Username { get; init; }
    public bool IsAuthenticated { get; init; }
}
