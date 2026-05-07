using MasarHub.Domain.Modules.Profiles;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using MasarHub.Infrastructure.Persistence.Identity;
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
                   .HasColumnType("nvarchar")
                   .HasMaxLength(2000)
                   .IsRequired(false);

            builder.Property(i => i.Company)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(i => i.Headline)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(100)
                   .HasMaxLength(200);

            builder.Property(x => x.VerificationStatus)
                .HasConversion<string>()
                .HasColumnType("nvarchar")
                .HasMaxLength(20)
                .IsRequired();


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