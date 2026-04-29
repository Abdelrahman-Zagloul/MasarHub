using MasarHub.Domain.Modules.Orders;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Orders
{
    internal sealed class OrderConfiguration : SoftDeletableEntityConfiguration<Order>, IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            ConfigureSoftDelete(builder);

            builder.ToTable("Orders", "orders", tb =>
            {
                tb.HasCheckConstraint("CK_Orders_FinalAmount_NonNegative", "[FinalAmount] >= 0");
            });

            builder.Property(o => o.UserId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(o => o.FinalAmount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(o => o.Status)
                   .HasConversion<string>()
                   .HasColumnType("nvarchar")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(o => o.OrderNumber)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(o => o.CreatedAt)
                   .IsRequired();

            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => o.Status);
            builder.HasIndex(o => o.OrderNumber).IsUnique();

            ConfigureOrderItems(builder);
        }
        private static void ConfigureOrderItems(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsMany(o => o.Items, items =>
            {
                items.ToTable("OrderItems", "orders", tb =>
                {
                    tb.HasCheckConstraint("CK_OrderItems_OriginalPrice_NonNegative", "[OriginalPrice] >= 0");
                    tb.HasCheckConstraint("CK_OrderItems_FinalPrice_NonNegative", "[FinalPrice] >= 0");
                    tb.HasCheckConstraint("CK_OrderItems_Discount_Valid", "[DiscountAmount] <= [OriginalPrice]");
                });

                items.WithOwner().HasForeignKey("OrderId");

                items.Property<int>("Id");
                items.HasKey("OrderId", "Id");

                items.Property(i => i.CourseId)
                     .HasColumnType("uniqueidentifier")
                     .IsRequired();

                items.Property(i => i.CourseTitle)
                     .HasColumnType("nvarchar")
                     .HasMaxLength(250)
                     .IsRequired();

                items.Property(i => i.OriginalPrice)
                     .HasPrecision(18, 2)
                     .IsRequired();

                items.Property(i => i.DiscountAmount)
                     .HasPrecision(18, 2)
                     .IsRequired();

                items.Property(i => i.FinalPrice)
                     .HasPrecision(18, 2)
                     .IsRequired();

                items.Property(i => i.CouponId)
                     .HasColumnType("uniqueidentifier")
                     .IsRequired(false);

                items.HasIndex("OrderId", nameof(OrderItem.CourseId))
                     .IsUnique();
            });

            builder.Navigation(o => o.Items)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
