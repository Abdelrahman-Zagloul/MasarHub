using MasarHub.Domain.Modules.Exams;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Exams
{
    internal sealed class QuestionConfiguration
        : BaseEntityConfiguration<Question>, IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            ConfigureBase(builder);

            builder.ToTable("Questions", "exams", tb =>
            {
                tb.HasCheckConstraint("CK_Questions_Mark_Positive",
                    "[QuestionMark] > 0");
            });

            builder.Property(q => q.ExamId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(q => q.QuestionText)
                   .HasColumnType("nvarchar(1000)")
                   .HasMaxLength(1000)
                   .IsRequired();

            builder.Property(q => q.QuestionMark)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(q => q.QuestionType)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.HasOne<Exam>()
                   .WithMany(e => e.Questions)
                   .HasForeignKey(q => q.ExamId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.Options)
                   .WithOne()
                   .HasForeignKey(o => o.QuestionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(q => q.Options)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(q => q.ExamId);
        }
    }
}