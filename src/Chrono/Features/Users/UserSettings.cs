using Chrono.Common;
using Chrono.Features.TaskLists;

namespace Chrono.Features.Users;

public class UserSettings : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }

    public TaskList DefaultTaskList { get; set; }
}
