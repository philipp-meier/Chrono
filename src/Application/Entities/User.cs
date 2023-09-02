using Chrono.Application.Entities.Common;

namespace Chrono.Application.Entities;

public class User : BaseEntity
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public UserSettings UserSettings { get; set; }
}
