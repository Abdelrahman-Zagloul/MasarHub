using MasarHub.Domain.Modules.Exams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Exams
{
    internal sealed class ExamAnswerOptionConfiguration : IEntityTypeConfiguration<ExamAnswerOption>
    {
        public void Configure(EntityTypeBuilder<ExamAnswerOption> builder)
        {
            builder.ToTable("ExamAnswerOptions", "exams");

            builder.HasKey(x => new { x.ExamAnswerId, x.OptionId });


            builder.Property(x => x.ExamAnswerId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(x => x.OptionId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.HasOne<ExamAnswer>()
                   .WithMany("_selectedOptions")
                   .HasForeignKey(x => x.ExamAnswerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Option>()
                   .WithMany()
                   .HasForeignKey(x => x.OptionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.OptionId);
            builder.HasIndex(x => x.ExamAnswerId);
        }
    }
}