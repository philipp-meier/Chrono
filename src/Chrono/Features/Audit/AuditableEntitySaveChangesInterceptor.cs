using Chrono.Common;
using Chrono.Entities;
using Chrono.Entities.Common;
using Chrono.Features.Users;
using Chrono.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Audit;

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
            currentUser = new User
            {
                UserId = _currentUserService.UserId, Name = _currentUserService.UserName
            };
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