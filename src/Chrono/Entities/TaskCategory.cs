using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chrono.Entities;

public sealed class TaskCategory
{
    public int TaskId { get; set; }
    public Task Task { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }
}

internal sealed class TaskCategoryConfiguration : IEntityTypeConfiguration<TaskCategory>
{
    public void Configure(EntityTypeBuilder<TaskCategory> builder)
    {
        builder.HasKey(x => new
        {
            x.TaskId, x.CategoryId
        });

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
