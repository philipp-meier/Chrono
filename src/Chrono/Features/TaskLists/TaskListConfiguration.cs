using Chrono.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chrono.Features.TaskLists;

public class TaskListConfiguration : IEntityTypeConfiguration<TaskList>
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
