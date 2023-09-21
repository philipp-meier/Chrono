using Chrono.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chrono.Entities;

public sealed class Task : BaseAuditableEntity
{
    public int ListId { get; set; }
    public int Position { get; set; }
    public string Name { get; set; }
    public string BusinessValue { get; set; }
    public string Description { get; set; }
    public bool Done { get; set; }
    public IList<TaskCategory> Categories { get; } = new List<TaskCategory>();
    public TaskList List { get; set; }
}

internal sealed class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Position)
            .IsRequired();

        builder.Navigation(x => x.CreatedBy)
            .AutoInclude();

        builder.Navigation(x => x.LastModifiedBy)
            .AutoInclude();
    }
}
