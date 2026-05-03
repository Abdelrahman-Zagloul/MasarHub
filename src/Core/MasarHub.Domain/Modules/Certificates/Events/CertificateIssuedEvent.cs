using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Certificates.Events
{
    public sealed record CertificateIssuedEvent(Guid CertificateId, Guid UserId, Guid CourseId) : DomainEvent;
}
