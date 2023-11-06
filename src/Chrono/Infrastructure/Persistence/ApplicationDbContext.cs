using System.Reflection;
using Chrono.Shared.Audit;
using Chrono.Shared.Interfaces;
using Chrono.Entities;
using Chrono.Features.TaskLists;
using Chrono.Features.Tasks;
using Microsoft.EntityFrameworkCore;
using Task = Chrono.Entities.Task;

namespace Chrono.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    private readonly TaskListSaveChangesInterceptor _taskListSaveChangesInterceptor;
    private readonly TaskSaveChangesInterceptor _taskSaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
        TaskSaveChangesInterceptor taskSaveChangesInterceptor,
        TaskListSaveChangesInterceptor taskListSaveChangesInterceptor)
        : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        _taskSaveChangesInterceptor = taskSaveChangesInterceptor;
        _taskListSaveChangesInterceptor = taskListSaveChangesInterceptor;
    }

    public DbSet<Task> Tasks => Set<Task>();
    public DbSet<TaskList> TaskLists => Set<TaskList>();
    public DbSet<TaskListOptions> TaskListOptions => Set<TaskListOptions>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<TaskCategory> TaskCategories => Set<TaskCategory>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
        optionsBuilder.AddInterceptors(_taskSaveChangesInterceptor);
        optionsBuilder.AddInterceptors(_taskListSaveChangesInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
}
