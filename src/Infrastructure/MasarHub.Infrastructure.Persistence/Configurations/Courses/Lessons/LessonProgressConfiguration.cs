using MasarHub.Domain.Modules.Courses.Lessons;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using MasarHub.Infrastructure.Persistence.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Courses.Lessons
{
    internal sealed class LessonProgressConfiguration
      : SoftDeletableEntityConfiguration<LessonProgress>, IEntityTypeConfiguration<LessonProgress>
    {
        public void Configure(EntityTypeBuilder<LessonProgress> builder)
        {
            ConfigureSoftDelete(builder);

            builder.ToTable("LessonProgress", "courses", tb =>
            {
                tb.HasCheckConstraint(
                    "CK_LessonProgress_CompletedAt_WhenCompleted",
                    "[IsCompleted] = 0 OR [CompletedAt] IS NOT NULL");
            });

            builder.Property(p => p.UserId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(p => p.LessonId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(p => p.ModuleId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(p => p.CourseId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(p => p.IsCompleted)
                   .IsRequired();

            builder.Property(p => p.CompletedAt)
                   .IsRequired(false);

            builder.HasOne<Lesson>()
                   .WithMany()
                   .HasForeignKey(p => p.LessonId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasIndex(p => p.CourseId);
            builder.HasIndex(p => p.ModuleId);
            builder.HasIndex(p => new { p.UserId, p.LessonId })
                   .IsUnique();


        }
    }
}