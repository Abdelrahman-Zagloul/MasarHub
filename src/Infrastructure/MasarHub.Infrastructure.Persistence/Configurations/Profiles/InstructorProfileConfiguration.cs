using MasarHub.Domain.Modules.Profiles;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using MasarHub.Infrastructure.Persistence.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Profiles
{
    internal sealed class InstructorProfileConfiguration
        : BaseEntityConfiguration<InstructorProfile>, IEntityTypeConfiguration<InstructorProfile>
    {
        public void Configure(EntityTypeBuilder<InstructorProfile> builder)
        {
            ConfigureBase(builder);

            builder.ToTable("InstructorProfiles", "users");

            builder.Property(i => i.UserId)
                   .IsRequired();

            builder.Property(i => i.Bio)
                   .HasMaxLength(2000)
                   .IsRequired();

            builder.Property(i => i.Headline)
                   .HasMaxLength(200);

            builder.HasOne<ApplicationUser>()
                   .WithOne()
                   .HasForeignKey<InstructorProfile>(i => i.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(i => i.UserId).IsUnique();

            builder.OwnsMany(i => i.SocialLinks, sl =>
            {
                sl.ToTable("InstructorSocialLinks", "users");

                sl.WithOwner().HasForeignKey("InstructorProfileId");

                sl.Property<int>("Id"); // shadow key
                sl.HasKey("Id");

                sl.Property(x => x.Platform)
                  .HasMaxLength(50)
                  .IsRequired();

                sl.Property(x => x.Url)
                  .HasMaxLength(500)
                  .IsRequired();

                sl.HasIndex("InstructorProfileId", nameof(SocialLink.Url))
                  .IsUnique();
            });
        }
    }
}