using Chrono.Application.Entities.Common;

namespace Chrono.Application.Entities;

public class TaskList : BaseAuditableEntity
{
    public string Title { get; set; }
    public IList<Task> Tasks { get; set; } = new List<Task>();
    public TaskListOptions Options { get; set; }
}
