using Chrono.Entities;
using Microsoft.EntityFrameworkCore;
using Task = Chrono.Entities.Task;

namespace Chrono.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Task> Tasks { get; }
    DbSet<TaskList> TaskLists { get; }
    DbSet<TaskListOptions> TaskListOptions { get; }
    DbSet<Category> Categories { get; }
    DbSet<TaskCategory> TaskCategories { get; }
    DbSet<Note> Notes { get; }
    DbSet<User> Users { get; }
    DbSet<UserSettings> UserSettings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
