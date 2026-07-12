using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface IAnnouncementQuery : IScopedService
    {
        Task<List<Guid>> GetEnrolledUserIdsAsync(Guid courseId, CancellationToken ct = default);
        Task<InstructorCourseAnnouncementResponse?> GetByIdForInstructorAsync(Guid courseId, Guid announcementId, Guid instructorId, CancellationToken ct = default);
    }
}
