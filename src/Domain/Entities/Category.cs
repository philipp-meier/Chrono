namespace Chrono.Domain.Entities;

public class Category : BaseAuditableEntity
{
    public string Name { get; set; }
    public IEnumerable<TaskCategory> Tasks { get; } = new List<TaskCategory>();
}
