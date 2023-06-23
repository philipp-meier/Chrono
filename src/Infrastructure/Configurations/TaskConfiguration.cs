using Microsoft.EntityFrameworkCore;
using Task = Chrono.Domain.Entities.Task;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chrono.Infrastructure.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.BusinessValue)
            .IsRequired();

        builder.Property(x => x.Position)
            .IsRequired();

        builder.Navigation(x => x.CreatedBy)
            .AutoInclude();

        builder.Navigation(x => x.LastModifiedBy)
            .AutoInclude();
    }
}
