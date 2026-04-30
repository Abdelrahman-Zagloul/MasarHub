using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Exams;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Exams
{
    internal sealed class ExamConfiguration
        : SoftDeletableEntityConfiguration<Exam>, IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            ConfigureSoftDelete(builder);

            builder.ToTable("Exams", "exams", tb =>
            {
                tb.HasCheckConstraint("CK_Exams_PassingScore_Range", "[PassingScorePercentage] >= 0 AND [PassingScorePercentage] <= 100");
                tb.HasCheckConstraint("CK_Exams_Duration_Positive", "[DurationInMinutes] IS NULL OR [DurationInMinutes] > 0");
                tb.HasCheckConstraint("CK_Exams_MaxAttempts_Positive", "[MaxAttempts] > 0");
            });

            builder.Property(e => e.CourseId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(e => e.ModuleId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired(false);

            builder.Property(e => e.Title)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(250)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(2000)
                   .IsRequired(false);

            builder.Property(e => e.PassingScorePercentage)
                   .IsRequired();

            builder.Property(e => e.DurationInMinutes)
                   .IsRequired(false);

            builder.Property(e => e.MaxAttempts)
                   .IsRequired();

            builder.Property(e => e.IsPublished)
                   .IsRequired();

            builder.HasOne<Course>()
                   .WithMany()
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<CourseModule>()
                   .WithMany()
                   .HasForeignKey(e => e.ModuleId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasMany(e => e.Questions)
                   .WithOne()
                   .HasForeignKey(q => q.ExamId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(e => e.Questions)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(e => e.CourseId);
            builder.HasIndex(e => e.ModuleId);
        }
    }
}
