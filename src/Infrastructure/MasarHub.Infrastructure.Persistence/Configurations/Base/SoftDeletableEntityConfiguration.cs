using MasarHub.Domain.SharedKernel.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Base
{
    internal abstract class SoftDeletableEntityConfiguration<TEntity>
        : BaseEntityConfiguration<TEntity> where TEntity : SoftDeletableEntity
    {
        protected void ConfigureSoftDelete(EntityTypeBuilder<TEntity> builder)
        {
            ConfigureBase(builder);

            builder.Property(e => e.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(e => e.DeletedAt)
                   .IsRequired(false);

            builder.HasIndex(e => e.IsDeleted);
        }
    }
}
