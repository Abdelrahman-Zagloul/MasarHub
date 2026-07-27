using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Profiles.Events
{
    public sealed record InstructorApprovedDomainEvent(Guid ApprovedByAdminId, Guid InstructorUserId) : DomainEvent;
}
