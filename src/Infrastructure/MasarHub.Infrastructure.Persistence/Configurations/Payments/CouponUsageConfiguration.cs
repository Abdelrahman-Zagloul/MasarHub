using MasarHub.Domain.Modules.Payments;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using MasarHub.Infrastructure.Persistence.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Payments
{
    internal sealed class CouponUsageConfiguration : BaseEntityConfiguration<CouponUsage>, IEntityTypeConfiguration<CouponUsage>
    {
        public void Configure(EntityTypeBuilder<CouponUsage> builder)
        {
            ConfigureBase(builder);

            builder.ToTable("CouponUsages", "payments");

            builder.Property(c => c.CouponId)
                   .IsRequired();

            builder.Property(c => c.UserId)
                   .IsRequired();

            builder.Property(c => c.UsedAt)
                   .IsRequired();

            builder.HasOne<Coupon>()
                   .WithMany()
                   .HasForeignKey(c => c.CouponId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => new { c.CouponId, c.UserId })
                   .IsUnique();

            builder.HasIndex(c => c.UserId);
            builder.HasIndex(c => c.CouponId);
        }
    }
}