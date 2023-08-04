using System.Reflection;
using Chrono.Application.Common.Interfaces;
using Chrono.Application.Entities;
using Chrono.Application.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Task = Chrono.Application.Entities.Task;

namespace Chrono.Application.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    private readonly TaskSaveChangesInterceptor _taskSaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
        TaskSaveChangesInterceptor taskSaveChangesInterceptor)
        : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        _taskSaveChangesInterceptor = taskSaveChangesInterceptor;
    }

    public DbSet<Task> Tasks => Set<Task>();
    public DbSet<TaskList> TaskLists => Set<TaskList>();
    public DbSet<TaskListOptions> TaskListOptions => Set<TaskListOptions>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<TaskCategory> TaskCategories => Set<TaskCategory>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
        optionsBuilder.AddInterceptors(_taskSaveChangesInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
}
