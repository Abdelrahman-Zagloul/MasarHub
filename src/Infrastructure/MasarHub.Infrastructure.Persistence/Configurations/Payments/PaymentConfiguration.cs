using MasarHub.Domain.Modules.Payments;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using MasarHub.Infrastructure.Persistence.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Payments
{
    internal sealed class PaymentConfiguration
        : BaseEntityConfiguration<Payment>, IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            ConfigureBase(builder);

            builder.ToTable("Payments", "payments", tb =>
            {
                tb.HasCheckConstraint("CK_Payments_Amount_Positive", "[Amount] > 0");
                tb.HasCheckConstraint("CK_Payments_ExternalId_NotNull_WhenSucceeded",
                    "[Status] <> 'Succeeded' OR [ExternalId] IS NOT NULL");
            });


            builder.Property(p => p.UserId)
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

            builder.Property(p => p.ExternalId)
                   .HasMaxLength(200)
                   .IsRequired(false);

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);



            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.Provider);
            builder.HasIndex(p => new { p.Provider, p.ExternalId })
                   .IsUnique().HasFilter("[ExternalId] IS NOT NULL");
        }
    }
}