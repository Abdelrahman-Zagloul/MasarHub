using MasarHub.Domain.Modules.Exams;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Exams
{
    internal sealed class ExamAttemptConfiguration
        : SoftDeletableEntityConfiguration<ExamAttempt>, IEntityTypeConfiguration<ExamAttempt>
    {
        public void Configure(EntityTypeBuilder<ExamAttempt> builder)
        {
            ConfigureSoftDelete(builder);

            builder.ToTable("ExamAttempts", "exams", tb =>
            {
                tb.HasCheckConstraint("CK_ExamAttempts_Score_NonNegative", "[Score] >= 0");
                tb.HasCheckConstraint("CK_ExamAttempts_SubmittedAt_When_Submitted", "[Status] <> 'Submitted' OR [SubmittedAt] IS NOT NULL");
            });

            builder.Property(a => a.ExamId)
                    .HasColumnType("uniqueidentifier")
                    .IsRequired();

            builder.Property(a => a.UserId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(a => a.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(a => a.StartedAt)
                   .HasColumnType("datetimeoffset")
                   .IsRequired();

            builder.Property(a => a.SubmittedAt)
                   .HasColumnType("datetimeoffset")
                   .IsRequired(false);

            builder.Property(a => a.Score)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.HasOne<Exam>()
                   .WithMany()
                   .HasForeignKey(a => a.ExamId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Answers)
                   .WithOne()
                   .HasForeignKey(x => x.ExamAttemptId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(a => a.Answers)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(a => new { a.ExamId, a.UserId });
            builder.HasIndex(a => a.Status);
        }
    }
}
