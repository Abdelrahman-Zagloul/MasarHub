using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Certificates.Events;

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
            CertificateId = certificateId;
            TemplateId = templateId;
            DownloadedAt = DateTimeOffset.UtcNow;

            RaiseDomainEvent(new CertificateDownloadedEvent(certificateId));
        }

        public static Result<CertificateDownload> Create(Guid certificateId, Guid templateId)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(certificateId, nameof(certificateId)),
                Guard.AgainstEmptyGuid(templateId, nameof(templateId))
            );

            if (error is not null)
                return error;

            return new CertificateDownload(certificateId, templateId);
        }
    }
}