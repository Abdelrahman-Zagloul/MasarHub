using MasarHub.Domain.Modules.Orders;
using MasarHub.Domain.Modules.Payments;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Payments
{
    internal sealed class PaymentConfiguration : BaseEntityConfiguration<Payment>, IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            ConfigureBase(builder);

            builder.ToTable("Payments", "payments", tb =>
            {
                tb.HasCheckConstraint("CK_Payments_Amount_Positive", "[Amount] > 0");
                tb.HasCheckConstraint("CK_Payments_ProviderReference_NotNull_WhenSucceeded",
                    "[Status] <> 'Succeeded' OR [ProviderReference] IS NOT NULL");
            });

            builder.Property(p => p.OrderId)
                 .HasColumnType("uniqueidentifier")
                 .IsRequired();

            builder.Property(p => p.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(p => p.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(p => p.Provider)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(p => p.ProviderReference)
                   .HasMaxLength(200)
                   .IsRequired(false);

            builder.Property(p => p.PaidAt)
                   .HasColumnType("datetimeoffset")
                   .IsRequired(false);

            builder.HasOne<Order>()
                   .WithMany()
                   .HasForeignKey(p => p.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(p => p.OrderId);

            builder.HasIndex(p => new { p.Provider, p.ProviderReference })
                   .IsUnique().HasFilter("[ProviderReference] IS NOT NULL");
        }
    }
}