using MasarHub.Infrastructure.Persistence.Configurations.Base;
using MasarHub.Infrastructure.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Identity
{
    internal sealed class RefreshTokenConfiguration : BaseEntityConfiguration<RefreshToken>, IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            ConfigureBase(builder);

            builder.ToTable("RefreshTokens", "identity");

            builder.Property(t => t.TokenHash)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(t => t.ExpiresAt)
                   .IsRequired();

            builder.Property(t => t.CreatedByIp)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(t => t.RevokedByIp)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(t => t.RevokedAt);

            builder.Property(t => t.ReplacedByRefreshTokenId);

            builder.Property(t => t.UserId)
                   .IsRequired();

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasIndex(t => t.TokenHash)
                   .IsUnique();

            builder.HasIndex(t => t.UserId);

            //Self reference(rotation)
            builder.HasOne<RefreshToken>()
                   .WithMany()
                   .HasForeignKey(t => t.ReplacedByRefreshTokenId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}