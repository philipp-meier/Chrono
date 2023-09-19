using Chrono.Features.Audit;
using Chrono.Features.Categories;
using Chrono.Features.TaskLists;

namespace Chrono.Features.Tasks;

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

public static class TaskExtensions
{
    public static void SetCategories(this Task task, Category[] newCategories)
    {
        var currentCategories = task.Categories
            .Select(x => x.Category)
            .ToArray();

        foreach (var existing in currentCategories)
        {
            if (newCategories.Contains(existing))
            {
                continue;
            }

            var assignmentsToRemove = task.Categories.Where(x => x.Category == existing).ToArray();
            foreach (var toRemove in assignmentsToRemove)
            {
                task.Categories.Remove(toRemove);
            }
        }

        foreach (var newCategory in newCategories)
        {
            if (!currentCategories.Contains(newCategory))
            {
                task.Categories.Add(new TaskCategory
                {
                    Task = task, Category = newCategory
                });
            }
        }
    }
}
