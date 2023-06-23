using Chrono.Domain.Common;
using Chrono.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Chrono.Application.Common.Interfaces;

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
            return;

        var changedAuditableEntities = dbContext.ChangeTracker.Entries<BaseAuditableEntity>();
        var currentUser = dbContext.Users.FirstOrDefault(x => x.UserId == _currentUserService.UserId);
        var currentUtcDate = DateTime.UtcNow;

        foreach (var entry in changedAuditableEntities)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = currentUser;
                entry.Entity.Created = currentUtcDate;
            }

            if (entry.State is not (EntityState.Added or EntityState.Modified))
                continue;

            entry.Entity.LastModifiedBy = currentUser;
            entry.Entity.LastModified = currentUtcDate;
        }

        var changedTaskCategories = context.ChangeTracker.Entries<TaskCategory>();
        foreach (var entry in changedTaskCategories)
        {
            if (entry.State is not (EntityState.Added or EntityState.Deleted))
                continue;

            var taskToUpdate = entry.Entity.Task;
            if (taskToUpdate == null && entry.OriginalValues.TryGetValue<int>(nameof(TaskCategory.TaskId), out var prevTaskId))
                taskToUpdate = context.Find<Domain.Entities.Task>(prevTaskId);

            taskToUpdate.LastModifiedBy = currentUser;
            taskToUpdate.LastModified = currentUtcDate;
        }
    }
}
