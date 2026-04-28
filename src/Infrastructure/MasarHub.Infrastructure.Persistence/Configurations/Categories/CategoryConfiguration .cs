using MasarHub.Domain.Modules.Categories;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Categories
{
    internal sealed class CategoryConfiguration : BaseEntityConfiguration<Category>, IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            ConfigureBase(builder);

            builder.ToTable("Categories", "categories", tb =>
            {
                tb.HasCheckConstraint("CK_Categories_LevelMax", "[Level] >= 1 AND [Level] <= 3");
                tb.HasCheckConstraint("CK_Categories_DisplayOrder_Positive", "[DisplayOrder] > 0");

            });

            builder.Property(c => c.Name)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(c => c.Slug)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(c => c.Level)
                   .IsRequired();

            builder.Property(c => c.DisplayOrder)
                   .IsRequired();

            builder.Property(c => c.ParentCategoryId)
                   .IsRequired(false);

            builder.HasOne<Category>()
                   .WithMany()
                   .HasForeignKey(c => c.ParentCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.ParentCategoryId);
            builder.HasIndex(c => new { c.ParentCategoryId, c.Slug })
                .IsUnique();

        }
    }
}