using MasarHub.Domain.Modules.Exams;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Exams
{
    internal sealed class OptionConfiguration
        : BaseEntityConfiguration<Option>, IEntityTypeConfiguration<Option>
    {
        public void Configure(EntityTypeBuilder<Option> builder)
        {
            ConfigureBase(builder);

            builder.ToTable("Options", "exams");

            builder.Property(o => o.QuestionId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(o => o.Text)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(o => o.IsCorrect)
                   .IsRequired();


            builder.HasOne<Question>()
                   .WithMany(q => q.Options)
                   .HasForeignKey(o => o.QuestionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.QuestionId);
        }
    }
}