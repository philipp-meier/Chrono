using Chrono.Entities;
using Task = Chrono.Entities.Task;

namespace Chrono.Features.Tasks;

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

            foreach (var toRemove in task.Categories.Where(x => x.Category == existing))
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
