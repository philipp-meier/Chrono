using Chrono.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chrono.Entities;

public sealed class Note : BaseAuditableEntity
{
    public string Title { get; set; }
    public string Text { get; set; }
}

internal sealed class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.Property(x => x.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Text)
            .IsRequired();

        builder.Navigation(x => x.CreatedBy)
            .AutoInclude();
    }
}
