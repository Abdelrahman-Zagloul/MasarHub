using MasarHub.Domain.Modules.Exams;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Exams
{
    internal sealed class ExamAnswerConfiguration
        : SoftDeletableEntityConfiguration<ExamAnswer>, IEntityTypeConfiguration<ExamAnswer>
    {
        public void Configure(EntityTypeBuilder<ExamAnswer> builder)
        {
            ConfigureSoftDelete(builder);

            builder.ToTable("ExamAnswers", "exams");

            builder.Property(a => a.ExamAttemptId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(a => a.QuestionId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(a => a.AnsweredAt)
                   .HasColumnType("datetimeoffset")
                   .IsRequired();

            builder.HasOne<ExamAttempt>()
                   .WithMany(a => a.Answers)
                   .HasForeignKey(a => a.ExamAttemptId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Question>()
                   .WithMany()
                   .HasForeignKey(a => a.QuestionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<ExamAnswerOption>("_selectedOptions")
                   .WithOne()
                   .HasForeignKey(o => o.ExamAnswerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation("_selectedOptions")
                   .UsePropertyAccessMode(PropertyAccessMode.Field);


            builder.HasIndex(a => a.ExamAttemptId);

            builder.HasIndex(a => a.QuestionId);

            builder.HasIndex(a => new { a.ExamAttemptId, a.QuestionId })
                   .IsUnique();
        }
    }
}