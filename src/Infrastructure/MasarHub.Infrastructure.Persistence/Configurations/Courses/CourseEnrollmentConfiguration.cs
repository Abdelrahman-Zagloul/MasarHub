using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Payments;
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
                tb.HasCheckConstraint("CK_CourseEnrollments_Payment_Consistency",
                    "([PaidAmount] = 0 AND [PaymentId] IS NULL) OR ([PaidAmount] > 0 AND [PaymentId] IS NOT NULL)"
                );
            });

            builder.Property(e => e.UserId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(e => e.CourseId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(e => e.PaidAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(e => e.PaymentId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired(false);

            builder.Property(e => e.EnrolledAt)
                   .IsRequired();

            builder.HasOne<Course>()
                   .WithMany()
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Payment>()
               .WithOne()
               .HasForeignKey<CourseEnrollment>(e => e.PaymentId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.CourseId);

            builder.HasIndex(e => new { e.UserId, e.CourseId })
                   .IsUnique();
        }
    }
}
