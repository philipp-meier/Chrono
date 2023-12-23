using System.Security.Claims;

namespace Chrono.Shared.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string UserId => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    public string UserName => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? "Visitor";

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
