using Chrono.Entities;
using Chrono.Infrastructure.Persistence;
using Chrono.Shared;
using Chrono.Shared.Extensions;
using Chrono.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Task = Chrono.Entities.Task;

namespace Chrono.Features.Tasks;

public class TaskSaveChangesInterceptor(ICurrentUserService currentUserService) : BaseSaveChangesInterceptor
{
    protected override void UpdateEntities(DbContext context)
    {
        if (context is not ApplicationDbContext dbContext)
        {
            return;
        }

        var currentUtcDate = DateTime.UtcNow;
        var currentUser = dbContext.Users.SingleOrDefault(x => x.UserId == currentUserService.UserId);

        HandleTaskPositionChanges(context);
        HandleTaskCategoryChanges(context, currentUser, currentUtcDate);
    }

    private static void HandleTaskPositionChanges(DbContext context)
    {
        var changedTasks = context.ChangeTracker
            .Entries<Task>()
            .ToArray();

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
            taskList.ReorderTaskPositions();
        }
    }

    private static void HandleTaskCategoryChanges(DbContext context, User currentUser, DateTime currentUtcDate)
    {
        var changedTaskCategories = context.ChangeTracker.Entries<TaskCategory>();
        foreach (var entry in changedTaskCategories)
        {
            if (entry.State is not (EntityState.Added or EntityState.Deleted))
            {
                continue;
            }

            var taskToUpdate = entry.Entity.Task;
            if (taskToUpdate == null &&
                entry.OriginalValues.TryGetValue<int>(nameof(TaskCategory.TaskId), out var prevTaskId))
            {
                taskToUpdate = context.Find<Task>(prevTaskId);
            }

            taskToUpdate!.LastModifiedBy = currentUser;
            taskToUpdate.LastModified = currentUtcDate;
        }
    }
}
