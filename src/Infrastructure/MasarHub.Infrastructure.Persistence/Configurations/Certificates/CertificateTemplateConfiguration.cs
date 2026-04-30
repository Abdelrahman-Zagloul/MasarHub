using MasarHub.Domain.Modules.Certificates;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Certificates
{
    internal sealed class CertificateTemplateConfiguration
        : BaseEntityConfiguration<CertificateTemplate>, IEntityTypeConfiguration<CertificateTemplate>
    {
        public void Configure(EntityTypeBuilder<CertificateTemplate> builder)
        {
            ConfigureBase(builder);

            builder.ToTable("CertificateTemplates", "certificates");

            builder.Property(t => t.Name)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(t => t.HtmlContent)
                   .HasColumnType("nvarchar(max)")
                   .IsRequired();

            builder.Property(t => t.PreviewImageUrl)
                   .HasMaxLength(500)
                   .IsRequired();

            builder.HasIndex(t => t.Name).IsUnique();
        }
    }
}