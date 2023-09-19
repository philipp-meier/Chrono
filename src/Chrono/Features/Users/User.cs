using Chrono.Common;

namespace Chrono.Features.Users;

public class User : BaseEntity
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public UserSettings UserSettings { get; set; }
}
