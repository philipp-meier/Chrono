using Chrono.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chrono.Entities;

public sealed class Category : BaseAuditableEntity
{
    public string Name { get; set; }
    public IEnumerable<TaskCategory> Tasks { get; } = new List<TaskCategory>();
}

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Navigation(x => x.CreatedBy)
            .AutoInclude();
    }
}
