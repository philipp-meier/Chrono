namespace Chrono.Shared.Services;

public class FakeCurrentUserService : ICurrentUserService
{
    public string UserId => "Testing";
    public string UserName => "Testing";
    public bool IsAuthenticated => true;
}
