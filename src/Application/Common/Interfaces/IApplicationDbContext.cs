using Chrono.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = Chrono.Domain.Entities.Task;

namespace Chrono.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Task> Tasks { get; }
    DbSet<TaskList> TaskLists { get; }
    DbSet<TaskListOptions> TaskListOptions { get; }
    DbSet<Category> Categories { get; }
    DbSet<TaskCategory> TaskCategories { get; }
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
