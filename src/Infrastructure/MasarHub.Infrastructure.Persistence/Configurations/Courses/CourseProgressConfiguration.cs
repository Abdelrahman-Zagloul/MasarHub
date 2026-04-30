using MasarHub.Domain.Modules.Courses;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Courses
{
    internal class CourseProgressConfiguration
        : SoftDeletableEntityConfiguration<CourseProgress>, IEntityTypeConfiguration<CourseProgress>
    {
        public void Configure(EntityTypeBuilder<CourseProgress> builder)
        {
            ConfigureSoftDelete(builder);

            builder.ToTable("CourseProgress", "courses", tb =>
            {
                tb.HasCheckConstraint("CK_CourseProgress_CompletedLessons_NonNegative", "[CompletedLessons] >= 0");
                tb.HasCheckConstraint("CK_CourseProgress_TotalLessons_Positive", "[TotalLessons] > 0");
                tb.HasCheckConstraint("CK_CourseProgress_TotalLessonsLessThanCompleted", "[TotalLessons] >= [CompletedLessons]");
                tb.HasCheckConstraint("CK_CourseProgress_Percentage_Range", "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");

            });

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.CourseId)
                .IsRequired();

            builder.Property(x => x.CompletedLessons)
                .IsRequired();

            builder.Property(x => x.TotalLessons)
                .IsRequired();

            builder.Property(x => x.ProgressPercentage)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(x => x.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.CompletedAt);

            builder.HasOne<Course>()
                .WithMany()
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasIndex(x => new { x.UserId, x.CourseId })
                .IsUnique();

            builder.HasIndex(x => x.CourseId);
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.IsCompleted);
        }
    }
}