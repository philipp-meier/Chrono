using System.Security.Claims;
using Chrono.Application.Common.Interfaces;

namespace Chrono.WebUI.Services;

public class CurrentUserService : ICurrentUserService
{
    public string UserId => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    public string UserName => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? "Visitor";

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
}
