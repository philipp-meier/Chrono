using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = Chrono.Entities.Task;

namespace Chrono.Features.Tasks;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
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