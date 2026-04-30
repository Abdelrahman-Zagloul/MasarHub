using MasarHub.Domain.Modules.Certificates;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Certificates
{
    internal sealed class CertificateDownloadConfiguration : BaseEntityConfiguration<CertificateDownload>, IEntityTypeConfiguration<CertificateDownload>
    {
        public void Configure(EntityTypeBuilder<CertificateDownload> builder)
        {
            ConfigureBase(builder);

            builder.ToTable("CertificateDownloads", "certificates");

            builder.Property(x => x.CertificateId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(x => x.TemplateId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(x => x.DownloadedAt)
                   .HasColumnType("datetimeoffset")
                   .IsRequired();

            builder.HasOne<Certificate>()
                   .WithMany()
                   .HasForeignKey(x => x.CertificateId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<CertificateTemplate>()
                   .WithMany()
                   .HasForeignKey(x => x.TemplateId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.CertificateId);
        }
    }
}