using MasarHub.Domain.Modules.Courses;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using MasarHub.Infrastructure.Persistence.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Courses
{
    internal sealed class CourseReviewConfiguration
         : SoftDeletableEntityConfiguration<CourseReview>, IEntityTypeConfiguration<CourseReview>
    {
        public void Configure(EntityTypeBuilder<CourseReview> builder)
        {
            ConfigureSoftDelete(builder);

            builder.ToTable("CourseReviews", "courses", tb =>
            {
                tb.HasCheckConstraint("CK_CourseReviews_Rating_Range", "[Rating] >= 1 AND [Rating] <= 5");
            });

            builder.Property(r => r.Rating)
                    .HasColumnType("decimal(3, 1)")
                   .IsRequired();

            builder.Property(r => r.ReviewContent)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(2000)
                   .IsRequired(false);

            builder.Property(r => r.UserId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(r => r.CourseId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(r => r.EditedAt)
                   .IsRequired(false);

            builder.HasOne<Course>()
                   .WithMany()
                   .HasForeignKey(r => r.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(r => r.CourseId);
            builder.HasIndex(r => new { r.UserId, r.CourseId })
                   .IsUnique();
        }
    }
}