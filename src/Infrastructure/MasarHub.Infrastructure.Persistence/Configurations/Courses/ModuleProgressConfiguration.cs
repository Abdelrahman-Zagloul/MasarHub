using MasarHub.Domain.Modules.Courses;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Courses
{
    internal class ModuleProgressConfiguration
        : SoftDeletableEntityConfiguration<ModuleProgress>, IEntityTypeConfiguration<ModuleProgress>
    {
        public void Configure(EntityTypeBuilder<ModuleProgress> builder)
        {
            ConfigureSoftDelete(builder);

            builder.ToTable("ModuleProgress", "courses", tb =>
            {
                tb.HasCheckConstraint("CK_ModuleProgress_CompletedLessons_NonNegative", "[CompletedLessons] >= 0");
                tb.HasCheckConstraint("CK_ModuleProgress_TotalLessons_Positive", "[TotalLessons] > 0");
                tb.HasCheckConstraint("CK_ModuleProgress_TotalLessonsLessThanCompleted", "[TotalLessons] >= [CompletedLessons]");
            });

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.CourseId)
                .IsRequired();

            builder.Property(x => x.ModuleId)
                .IsRequired();

            builder.Property(x => x.CompletedLessons)
                .IsRequired();

            builder.Property(x => x.TotalLessons)
                .IsRequired();

            builder.Property(x => x.CompletedAt);

            builder.HasOne<Course>()
                .WithMany()
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.UserId, x.ModuleId }).IsUnique();
            builder.HasIndex(x => new { x.UserId, x.CourseId });
            builder.HasIndex(x => x.CourseId);
            builder.HasIndex(x => x.ModuleId);
        }
    }
}
