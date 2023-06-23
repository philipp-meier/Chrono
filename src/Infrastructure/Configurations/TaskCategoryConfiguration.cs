using Chrono.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chrono.Infrastructure.Configurations;

public class TaskCategoryConfiguration : IEntityTypeConfiguration<TaskCategory>
{
    public void Configure(EntityTypeBuilder<TaskCategory> builder)
    {
        builder.HasKey(x => new { x.TaskId, x.CategoryId });
        builder.HasOne(x => x.Task)
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.TaskId)
            .HasPrincipalKey(x => x.Id);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.CategoryId)
            .HasPrincipalKey(x => x.Id);
    }
}
