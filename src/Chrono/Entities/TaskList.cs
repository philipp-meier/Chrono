using Chrono.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chrono.Entities;

public sealed class TaskList : BaseAuditableEntity
{
    public string Title { get; set; }
    public IList<Task> Tasks { get; set; } = new List<Task>();
    public TaskListOptions Options { get; set; }
}

internal sealed class TaskListConfiguration : IEntityTypeConfiguration<TaskList>
{
    public void Configure(EntityTypeBuilder<TaskList> builder)
    {
        builder.Property(x => x.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasMany(x => x.Tasks)
            .WithOne(x => x.List)
            .HasForeignKey(x => x.ListId)
            .HasPrincipalKey(x => x.Id);

        builder.HasOne(x => x.Options)
            .WithOne(x => x.TaskList)
            .HasPrincipalKey<TaskList>(x => x.Id)
            .HasForeignKey<TaskListOptions>(x => x.TaskListId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.Navigation(x => x.Options)
            .AutoInclude();

        builder.Navigation(x => x.CreatedBy)
            .AutoInclude();
    }
}
