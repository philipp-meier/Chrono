using Chrono.Entities;
using Chrono.Entities.Common;
using Chrono.Infrastructure.Persistence;
using Chrono.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Shared.Audit;

public class AuditableEntitySaveChangesInterceptor(ICurrentUserService currentUserService) : BaseSaveChangesInterceptor
{
    protected override void UpdateEntities(DbContext context)
    {
        if (context is not ApplicationDbContext dbContext)
        {
            return;
        }

        var currentUtcDate = DateTime.UtcNow;
        var currentUser = dbContext.Users.SingleOrDefault(x => x.UserId == currentUserService.UserId);
        if (currentUser == null && !string.IsNullOrEmpty(currentUserService.UserId))
        {
            currentUser = new User { UserId = currentUserService.UserId, Name = currentUserService.UserName };
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
    }
}
