using MasarHub.Infrastructure.Persistence.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Identity
{
    internal sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Users", "identity");

            builder.Property(x => x.FullName)
                .HasColumnType("nvarchar")
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
