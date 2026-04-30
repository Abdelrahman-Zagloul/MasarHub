using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;

namespace MasarHub.Domain.Modules.Certificates
{
    public sealed class CertificateDownload : BaseEntity
    {
        public Guid CertificateId { get; private set; }
        public Guid TemplateId { get; private set; }
        public DateTimeOffset DownloadedAt { get; private set; }
        private CertificateDownload() { }

        private CertificateDownload(Guid certificateId, Guid templateId)
        {
            CertificateId = Guard.AgainstEmptyGuid(certificateId, nameof(certificateId));
            TemplateId = Guard.AgainstEmptyGuid(templateId, nameof(templateId));
            DownloadedAt = DateTimeOffset.UtcNow;
        }

        public static CertificateDownload Create(Guid certificateId, Guid templateId)
            => new CertificateDownload(certificateId, templateId);
    }
}
