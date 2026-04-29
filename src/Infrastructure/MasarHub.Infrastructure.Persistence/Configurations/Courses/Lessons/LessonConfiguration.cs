using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Courses.Lessons;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Courses.Lessons
{
    internal sealed class LessonConfiguration
         : SoftDeletableEntityConfiguration<Lesson>, IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            ConfigureSoftDelete(builder);

            builder.ToTable("Lessons", "courses", tb =>
            {
                tb.HasCheckConstraint("CK_Lessons_DisplayOrder_Positive", "[DisplayOrder] > 0");
            });

            builder.HasDiscriminator<string>("LessonType")
                   .HasValue<VideoLesson>("video")
                   .HasValue<ArticleLesson>("article")
                   .HasValue<ResourceLesson>("resource");

            builder.Property(l => l.Title)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(l => l.Description)
                   .HasMaxLength(1000)
                   .IsRequired(false);

            builder.Property(l => l.DisplayOrder)
                   .IsRequired();

            builder.Property(l => l.IsPreviewable)
                   .IsRequired();

            builder.Property(l => l.ModuleId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.HasOne<CourseModule>()
                   .WithMany()
                   .HasForeignKey(l => l.ModuleId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(l => l.ModuleId);

            builder.HasIndex(l => new { l.ModuleId, l.DisplayOrder })
                   .IsUnique();

        }
    }
}