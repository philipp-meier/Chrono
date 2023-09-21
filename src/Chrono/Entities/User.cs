using Chrono.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chrono.Entities;

public sealed class User : BaseEntity
{
    public string UserId { get; set; }
    public string Name { get; set; }
    public UserSettings UserSettings { get; set; }
}

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.HasOne(x => x.UserSettings)
            .WithOne(x => x.User)
            .HasPrincipalKey<User>(x => x.Id)
            .HasForeignKey<UserSettings>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}
