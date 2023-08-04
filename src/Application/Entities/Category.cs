using Chrono.Application.Entities.Common;

namespace Chrono.Application.Entities;

public class Category : BaseAuditableEntity
{
    public string Name { get; set; }
    public IEnumerable<TaskCategory> Tasks { get; } = new List<TaskCategory>();
}
