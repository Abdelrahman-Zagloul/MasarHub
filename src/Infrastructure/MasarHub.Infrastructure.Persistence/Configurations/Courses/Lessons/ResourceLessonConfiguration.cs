using MasarHub.Domain.Modules.Courses.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Courses.Lessons
{
    internal sealed class ResourceLessonConfiguration : IEntityTypeConfiguration<ResourceLesson>
    {
        public void Configure(EntityTypeBuilder<ResourceLesson> builder)
        {
            builder.ToTable("Lessons", "courses", tb =>
            {
                tb.HasCheckConstraint(
                    "CK_ResourceLesson_FileSize_NonNegative",
                    "[FileSizeInBytes] >= 0");
            });
            builder.Property(r => r.ResourcePublicId)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(r => r.FileName)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(r => r.FileType)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(r => r.FileSizeInBytes)
                   .IsRequired();

        }
    }
}