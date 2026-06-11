using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Courses.Events
{
    public sealed record AttachmentDeletedDomainEvent(Guid AttachmentId) : DomainEvent;
}
