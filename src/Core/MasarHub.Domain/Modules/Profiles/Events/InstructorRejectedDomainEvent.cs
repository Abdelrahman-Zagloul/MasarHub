using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Profiles.Events
{
    public sealed record InstructorRejectedDomainEvent(Guid RejectedByAdminId, Guid InstructorUserId, string Reason) : DomainEvent;
}
