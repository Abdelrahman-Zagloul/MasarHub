using MasarHub.Domain.Common.Events;

namespace MasarHub.Domain.Modules.Courses.Events
{
    public sealed record ModuleCreatedDomainEvent(Guid CourseId, string ModuleTitle) : DomainEvent;
}
