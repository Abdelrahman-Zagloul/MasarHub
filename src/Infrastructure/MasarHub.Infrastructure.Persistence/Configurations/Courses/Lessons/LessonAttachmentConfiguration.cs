using MasarHub.Domain.Modules.Courses.Lessons;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Courses.Lessons
{
    internal sealed class LessonAttachmentConfiguration
          : SoftDeletableEntityConfiguration<LessonAttachment>, IEntityTypeConfiguration<LessonAttachment>
    {
        public void Configure(EntityTypeBuilder<LessonAttachment> builder)
        {
            ConfigureSoftDelete(builder);

            builder.ToTable("LessonAttachments", "courses", tb =>
            {
                tb.HasCheckConstraint(
                    "CK_LessonAttachments_FileSize_NonNegative",
                    "[FileSizeInBytes] >= 0");
            });

            builder.Property(a => a.PublicId)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(a => a.FileName)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(a => a.FileType)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(a => a.FileSizeInBytes)
                   .IsRequired();

            builder.Property(a => a.LessonId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.HasOne<Lesson>()
                   .WithMany()
                   .HasForeignKey(a => a.LessonId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => a.LessonId);

            builder.HasIndex(a => new { a.LessonId, a.PublicId })
                   .IsUnique();
        }
    }
}