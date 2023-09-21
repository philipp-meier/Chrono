using Chrono.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chrono.Entities;

public sealed class UserSettings : BaseEntity
{
    public int UserId { get; set; }
    public User User { get; set; }

    public TaskList DefaultTaskList { get; set; }
}

internal sealed class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        builder.Navigation(x => x.DefaultTaskList)
            .AutoInclude();
    }
}
