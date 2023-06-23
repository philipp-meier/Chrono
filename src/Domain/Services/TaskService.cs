using Task = Chrono.Domain.Entities.Task;

namespace Chrono.Domain.Services;

public static class TaskService
{
    public static void SetCategories(Task context, Category[] newCategories)
    {
        var currentCategories = context.Categories
            .Select(x => x.Category)
            .ToArray();

        foreach (var existing in currentCategories)
        {
            if (newCategories.Contains(existing))
                continue;

            var assignmentsToRemove = context.Categories.Where(x => x.Category == existing).ToArray();
            foreach (var toRemove in assignmentsToRemove)
                context.Categories.Remove(toRemove);
        }

        foreach (var newCategory in newCategories)
            if (!currentCategories.Contains(newCategory))
                context.Categories.Add(new TaskCategory { Task = context, Category = newCategory });
    }
}
