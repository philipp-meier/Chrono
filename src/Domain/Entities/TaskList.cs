namespace Chrono.Domain.Entities;

public class TaskList : BaseAuditableEntity
{
    public string Title { get; set; }
    public IList<Task> Tasks { get; set; } = new List<Task>();
}
