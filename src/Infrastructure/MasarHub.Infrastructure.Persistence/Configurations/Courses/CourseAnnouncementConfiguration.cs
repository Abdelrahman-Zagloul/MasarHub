using MasarHub.Domain.Modules.Courses;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Courses
{
    internal class CourseAnnouncementConfiguration
        : SoftDeletableEntityConfiguration<CourseAnnouncement>, IEntityTypeConfiguration<CourseAnnouncement>
    {
        public void Configure(EntityTypeBuilder<CourseAnnouncement> builder)
        {
            ConfigureSoftDelete(builder);
            builder.ToTable("CourseAnnouncements", "courses", tb =>
            {
                tb.HasCheckConstraint("CK_Announcement_PublishedAt_WhenPublished", "[IsPublished] = 0 OR [PublishedAt] IS NOT NULL");
            });

            builder.Property(e => e.CourseId)
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(e => e.InstructorId)
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(x => x.Title)
                .HasColumnType("nvarchar")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Content)
                .HasColumnType("nvarchar")
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(x => x.Importance)
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(x => x.IsPublished)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.IsPinned)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.ScheduledAt)
                .IsRequired(false);

            builder.Property(x => x.ExpiresAt)
                .IsRequired(false);

            builder.HasOne<Course>()
                .WithMany()
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.CourseId);
            builder.HasIndex(x => new { x.CourseId, x.IsPinned });
            builder.HasIndex(x => new { x.CourseId, x.IsPublished });
            builder.HasIndex(x => new { x.CourseId, x.IsPinned, x.Importance, x.PublishedAt });

        }
    }
}
