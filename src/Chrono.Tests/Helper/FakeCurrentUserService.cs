using Chrono.Shared.Services;

namespace Chrono.Tests.Helper;

public class FakeCurrentUserService : ICurrentUserService
{
    public string UserId => "Testing";
    public string UserName => "Testing";
    public bool IsAuthenticated => true;
}
