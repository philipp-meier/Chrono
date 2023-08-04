using Chrono.Application.Common.Services;
using Chrono.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Task = Chrono.Application.Entities.Task;

namespace Chrono.Application.Infrastructure.Persistence.Interceptors;

public class TaskSaveChangesInterceptor : BaseSaveChangesInterceptor
{
    protected override void UpdateEntities(DbContext context)
    {
        if (context == null)
        {
            return;
        }

        var changedTasks = context.ChangeTracker
            .Entries<Task>()
            .ToArray();

        UpdateTaskPositions(context, changedTasks);
    }

    private static void UpdateTaskPositions(DbContext context, IEnumerable<EntityEntry<Task>> changedTasks)
    {
        var taskListsToUpdate = new HashSet<TaskList>();
        foreach (var entry in changedTasks)
        {
            var positionEntry = entry.Property(x => x.Position);
            if (positionEntry.IsModified || entry.State == EntityState.Added)
            {
                taskListsToUpdate.Add(entry.Entity.List);
            }
            else if (entry.State == EntityState.Deleted &&
                     entry.OriginalValues.TryGetValue<int>(nameof(Task.ListId), out var prevListId))
            {
                var prevList = context.Find<TaskList>(prevListId);
                if (prevList != null)
                {
                    taskListsToUpdate.Add(prevList);
                }
            }
        }

        foreach (var taskList in taskListsToUpdate)
        {
            TaskListService.ReorderTaskPositions(taskList);
        }
    }
}
