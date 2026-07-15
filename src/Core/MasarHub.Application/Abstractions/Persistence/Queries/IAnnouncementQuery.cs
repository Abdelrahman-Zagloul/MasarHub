using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForStudent;
using MasarHub.Application.Features.Announcements.Queries.GetInstructorCourseAnnouncements;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface IAnnouncementQuery : IScopedService
    {
        Task<List<Guid>> GetEnrolledUserIdsAsync(Guid courseId, CancellationToken ct = default);
        Task<InstructorCourseAnnouncementResponse?> GetByIdForInstructorAsync(Guid courseId, Guid announcementId, Guid instructorId, CancellationToken ct = default);
        Task<StudentAnnouncementResult> GetByIdForStudentAsync(Guid courseId, Guid announcementId, Guid studentId, CancellationToken ct = default);
        Task<PagedResult<InstructorCourseAnnouncementResponse>> GetInstructorListAsync(GetInstructorCourseAnnouncementsQuery query, CancellationToken ct = default);
    }
    public sealed record StudentAnnouncementResult(bool IsEnrolled, StudentCourseAnnouncementResponse? Announcement);
}
