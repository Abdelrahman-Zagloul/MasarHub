using MasarHub.Domain.Modules.Notifications;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Notifications
{
    internal sealed class NotificationConfiguration
        : SoftDeletableEntityConfiguration<Notification>, IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            ConfigureSoftDelete(builder);

            builder.ToTable("Notifications", "notifications");

            builder.Property(x => x.TargetRole)
                .HasConversion<string>()
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasColumnType("nvarchar")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Message)
                .HasColumnType("nvarchar")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(x => x.Type)
                .HasConversion<string>()
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Priority)
                .HasConversion<string>()
                .HasColumnType("nvarchar")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.IsRead)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.ReadAt)
                .HasColumnType("datetimeoffset")
                .IsRequired(false);

            builder.Property(x => x.ActionUrl)
                .HasColumnType("nvarchar")
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(x => x.ResourceId)
                .HasColumnType("uniqueidentifier")
                .IsRequired(false);

            builder.HasIndex(x => x.TargetRole);
        }
    }
}
