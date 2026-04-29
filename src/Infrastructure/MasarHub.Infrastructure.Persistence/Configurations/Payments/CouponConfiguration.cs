using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Payments;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Payments
{
    internal sealed class CouponConfiguration
        : BaseEntityConfiguration<Coupon>, IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            ConfigureBase(builder);

            builder.ToTable("Coupons", "payments", tb =>
            {
                tb.HasCheckConstraint("CK_Coupons_Value_Positive", "[Value] > 0");

                tb.HasCheckConstraint("CK_Coupons_UsedCount_NonNegative", "[UsedCount] >= 0");
                tb.HasCheckConstraint("CK_Coupons_Percentage_Limit", "[Type] <> 'Percentage' OR [Value] <= 100");
            });

            builder.Property(c => c.Code)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(c => c.Value)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(c => c.Type)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(c => c.CourseId)
                   .IsRequired();

            builder.Property(c => c.ExpirationDate)
                   .IsRequired();

            builder.Property(c => c.UsageLimit)
                   .IsRequired();

            builder.Property(c => c.UsedCount)
                   .IsRequired();

            builder.HasOne<Course>()
                   .WithMany()
                   .HasForeignKey(c => c.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => c.Code)
                   .IsUnique();

            builder.HasIndex(c => c.CourseId);
        }
    }
}