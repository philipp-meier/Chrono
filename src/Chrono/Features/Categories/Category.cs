using Chrono.Features.Audit;
using Chrono.Features.Tasks;

namespace Chrono.Features.Categories;

public class Category : BaseAuditableEntity
{
    public string Name { get; set; }
    public IEnumerable<TaskCategory> Tasks { get; } = new List<TaskCategory>();
}
