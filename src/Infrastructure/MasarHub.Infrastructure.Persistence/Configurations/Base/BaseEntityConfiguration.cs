using MasarHub.Domain.SharedKernel.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Base
{
    internal abstract class BaseEntityConfiguration<TEntity> where TEntity : BaseEntity, IEntity
    {
        protected void ConfigureBase(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.CreatedAt)
                   .IsRequired();

            builder.Property(e => e.UpdatedAt)
                   .IsRequired(false);
        }
    }
}
