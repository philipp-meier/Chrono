using Chrono.Entities.Common;

namespace Chrono.Entities;

public class User : BaseEntity
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public UserSettings UserSettings { get; set; }
}
