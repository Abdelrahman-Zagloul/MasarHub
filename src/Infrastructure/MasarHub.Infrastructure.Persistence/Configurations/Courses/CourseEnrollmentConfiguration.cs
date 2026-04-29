using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Orders;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using MasarHub.Infrastructure.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Courses
{
    internal sealed class CourseEnrollmentConfiguration
         : SoftDeletableEntityConfiguration<CourseEnrollment>, IEntityTypeConfiguration<CourseEnrollment>
    {
        public void Configure(EntityTypeBuilder<CourseEnrollment> builder)
        {
            ConfigureSoftDelete(builder);

            builder.ToTable("CourseEnrollments", "courses", tb =>
            {
                tb.HasCheckConstraint("CK_CourseEnrollments_PaidAmount_NonNegative", "[PaidAmount] >= 0");
            });

            builder.Property(e => e.UserId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(e => e.CourseId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(e => e.OrderId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(e => e.PaidAmount)
                   .HasPrecision(18, 2)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(e => e.EnrolledAt)
                    .HasColumnType("datetimeoffset")
                    .IsRequired();

            builder.Property(e => e.CompletedAt)
                   .HasColumnType("datetimeoffset")
                   .IsRequired(false);

            builder.Property(e => e.Status)
               .HasConversion<string>()
               .HasMaxLength(50)
               .IsRequired();

            builder.HasOne<Course>()
               .WithMany()
               .HasForeignKey(e => e.CourseId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Order>()
                   .WithMany()
                   .HasForeignKey(e => e.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(e => e.CourseId);
            builder.HasIndex(e => e.OrderId);

            builder.HasIndex(e => new { e.UserId, e.CourseId })
                   .IsUnique();
        }
    }
}
