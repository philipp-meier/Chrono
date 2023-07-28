using Chrono.Application.Common.Interfaces;
using Chrono.Domain.Common;
using Chrono.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = Chrono.Domain.Entities.Task;

namespace Chrono.Infrastructure.Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor : BaseSaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public AuditableEntitySaveChangesInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    protected override void UpdateEntities(DbContext context)
    {
        if (context is not ApplicationDbContext dbContext)
        {
            return;
        }

        var currentUtcDate = DateTime.UtcNow;
        var currentUser = dbContext.Users.SingleOrDefault(x => x.UserId == _currentUserService.UserId);
        if (currentUser == null && !string.IsNullOrEmpty(_currentUserService.UserId))
        {
            currentUser = new User { UserId = _currentUserService.UserId, Name = _currentUserService.UserName };
            dbContext.Users.Add(currentUser);
        }

        foreach (var entry in dbContext.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = currentUser;
                entry.Entity.Created = currentUtcDate;
            }

            if (entry.State is not (EntityState.Added or EntityState.Modified))
            {
                continue;
            }

            entry.Entity.LastModifiedBy = currentUser;
            entry.Entity.LastModified = currentUtcDate;
        }

        HandleTaskCategoryChanges(context, currentUser, currentUtcDate);
        HandleTaskListOptionsChanges(context, currentUser, currentUtcDate);
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

    private static void HandleTaskListOptionsChanges(DbContext context, User currentUser, DateTime currentUtcDate)
    {
        var changedTaskListOptions = context.ChangeTracker.Entries<TaskListOptions>();
        foreach (var entry in changedTaskListOptions)
        {
            var taskListToUpdate = entry.Entity.TaskList;
            if (taskListToUpdate == null &&
                entry.OriginalValues.TryGetValue<int>(nameof(TaskListOptions.TaskListId), out var prevTaskListId))
            {
                taskListToUpdate = context.Find<TaskList>(prevTaskListId);
            }

            taskListToUpdate!.LastModifiedBy = currentUser;
            taskListToUpdate.LastModified = currentUtcDate;
        }
    }
}
