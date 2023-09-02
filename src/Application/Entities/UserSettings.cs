using Chrono.Application.Entities.Common;

namespace Chrono.Application.Entities;

public class UserSettings : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }

    public TaskList DefaultTaskList { get; set; }
}
