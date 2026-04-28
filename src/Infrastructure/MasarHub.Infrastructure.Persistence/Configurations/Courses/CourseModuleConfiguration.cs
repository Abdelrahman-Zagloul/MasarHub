using MasarHub.Domain.Modules.Courses;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Courses
{
    internal sealed class CourseModuleConfiguration : SoftDeletableEntityConfiguration<CourseModule>, IEntityTypeConfiguration<CourseModule>
    {
        public void Configure(EntityTypeBuilder<CourseModule> builder)
        {
            ConfigureSoftDelete(builder);

            builder.ToTable("CourseModules", "courses", tb =>
            {
                tb.HasCheckConstraint("CK_CourseModules_DisplayOrder_Positive", "[DisplayOrder] > 0");
            });


            builder.Property(m => m.Title)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(m => m.Description)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(2000)
                   .IsRequired(false);

            builder.Property(m => m.DisplayOrder)
                   .IsRequired();

            builder.Property(m => m.CourseId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.HasOne<Course>()
                   .WithMany()
                   .HasForeignKey(m => m.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => m.CourseId);
            builder.HasIndex(m => new { m.CourseId, m.DisplayOrder })
                   .IsUnique();

        }
    }
}
