using MasarHub.Domain.Common.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Base
{
    internal abstract class BaseEntityConfiguration<TEntity> where TEntity : BaseEntity, IEntity
    {
        protected void ConfigureBase(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Ignore(e => e.DomainEvents);

            builder.Property(e => e.CreatedAt)
                   .HasColumnType("datetimeoffset")
                   .IsRequired();

            builder.Property(e => e.UpdatedAt)
                   .HasColumnType("datetimeoffset")
                   .IsRequired(false);
        }
    }
}
