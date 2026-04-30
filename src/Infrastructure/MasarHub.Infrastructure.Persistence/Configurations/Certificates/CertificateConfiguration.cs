using MasarHub.Domain.Modules.Certificates;
using MasarHub.Domain.Modules.Courses;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using MasarHub.Infrastructure.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Certificates
{
    internal sealed class CertificateConfiguration
        : BaseEntityConfiguration<Certificate>, IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            ConfigureBase(builder);

            builder.ToTable("Certificates", "certificates");

            builder.Property(c => c.UserId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(c => c.CourseId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(c => c.EnrollmentId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(c => c.CertificateNumber)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(c => c.VerificationCode)
                .HasColumnType("nvarchar")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.IssuedAt)
                   .HasColumnType("datetimeoffset")
                   .IsRequired();

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Course>()
                   .WithMany()
                   .HasForeignKey(c => c.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<CourseEnrollment>()
                   .WithOne()
                   .HasForeignKey<Certificate>(c => c.EnrollmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<CertificateTemplate>()
                   .WithMany()
                   .HasForeignKey(c => c.TemplateId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => c.CertificateNumber).IsUnique();
            builder.HasIndex(c => c.VerificationCode).IsUnique();
            builder.HasIndex(c => c.EnrollmentId).IsUnique();
            builder.HasIndex(c => new { c.UserId, c.CourseId }).IsUnique();
        }
    }
}
