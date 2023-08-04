using Chrono.Application.Common.Interfaces;
using MediatR;

namespace Chrono.Application.Features.User;

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
