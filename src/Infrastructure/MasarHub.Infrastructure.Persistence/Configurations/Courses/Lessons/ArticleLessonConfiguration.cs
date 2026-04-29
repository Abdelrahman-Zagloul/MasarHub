using MasarHub.Domain.Modules.Courses.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Courses.Lessons
{
    internal sealed class ArticleLessonConfiguration : IEntityTypeConfiguration<ArticleLesson>
    {
        public void Configure(EntityTypeBuilder<ArticleLesson> builder)
        {
            builder.Property(a => a.Content)
                   .HasColumnType("nvarchar(max)")
                   .IsRequired();
        }
    }
}