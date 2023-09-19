using Chrono.Common;
using Chrono.Features.Users;
using Chrono.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.TaskLists;

public class TaskListSaveChangesInterceptor : BaseSaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public TaskListSaveChangesInterceptor(ICurrentUserService currentUserService)
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
