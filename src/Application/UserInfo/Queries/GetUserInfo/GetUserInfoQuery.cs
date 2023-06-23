using MediatR;
using Chrono.Application.Common.Interfaces;

namespace Chrono.Application.UserInfo.Queries.GetUserInfo;

public record GetUserInfoQuery : IRequest<UserInfoDto>;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, UserInfoDto>
{
    private readonly ICurrentUserService _currentUserService;

    public GetUserInfoQueryHandler(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<UserInfoDto> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(new UserInfoDto
        {
            Username = _currentUserService.UserName,
            IsAuthenticated = _currentUserService.IsAuthenticated
        });
    }
}
