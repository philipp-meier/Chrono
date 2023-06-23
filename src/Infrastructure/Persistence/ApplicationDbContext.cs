using System.Reflection;
using Chrono.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = Chrono.Domain.Entities.Task;
using Chrono.Application.Common.Interfaces;
using Chrono.Infrastructure.Persistence.Interceptors;

namespace Chrono.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<Task> Tasks => Set<Task>();
    public DbSet<TaskList> TaskLists => Set<TaskList>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<TaskCategory> TaskCategories => Set<TaskCategory>();
    public DbSet<User> Users => Set<User>();

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
