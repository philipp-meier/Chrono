using Chrono.Features.Categories;

namespace Chrono.Features.Tasks;

public class TaskCategory
{
    public int TaskId { get; set; }
    public Task Task { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }
}
