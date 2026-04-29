using MasarHub.Domain.Modules.Courses.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Courses.Lessons
{
    internal sealed class VideoLessonConfiguration : IEntityTypeConfiguration<VideoLesson>
    {
        public void Configure(EntityTypeBuilder<VideoLesson> builder)
        {
            builder.ToTable("Lessons", "courses", tb =>
            {
                tb.HasCheckConstraint("CK_VideoLesson_Duration_Positive", "[DurationInSeconds] > 0");
            });
            builder.Property(v => v.VideoPublicId)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(v => v.ThumbnailPublicId)
                   .HasMaxLength(500)
                   .IsRequired(false);

            builder.Property(v => v.DurationInSeconds)
                   .IsRequired();

        }
    }
}