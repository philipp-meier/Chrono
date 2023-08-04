using Chrono.Application.Entities.Common;

namespace Chrono.Application.Entities;

public class Task : BaseAuditableEntity
{
    public int ListId { get; set; }
    public int Position { get; set; }
    public string Name { get; set; }
    public string BusinessValue { get; set; }
    public string Description { get; set; }
    public bool Done { get; set; }
    public IList<TaskCategory> Categories { get; } = new List<TaskCategory>();
    public TaskList List { get; set; }
}
