using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Certificates.Events
{
    public sealed record CertificateDownloadedEvent(Guid CertificateId) : DomainEvent;
}
