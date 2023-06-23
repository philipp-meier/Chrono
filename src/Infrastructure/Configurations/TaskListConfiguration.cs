using Chrono.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chrono.Infrastructure.Configurations;

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
        
        builder.Navigation(x => x.CreatedBy)
            .AutoInclude();
    }
}
