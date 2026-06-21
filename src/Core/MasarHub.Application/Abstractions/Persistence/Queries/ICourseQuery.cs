using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Features.Courses.Queries.GetCourseById;
using MasarHub.Application.Features.Courses.Queries.GetCourses;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface ICourseQuery : IScopedService
    {
        Task<CourseCreationData> GetCreationDataAsync(string slug, Guid categoryId, CancellationToken ct = default);
        Task<InstructorInfo?> GetInstructorInfoAsync(Guid instructorId, CancellationToken ct = default);
        Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken ct = default);
        Task<bool> HasLecturesAsync(Guid courseId, CancellationToken ct = default);
        Task<CourseDetailsResponse?> GetDetailsByIdAsync(Guid courseId, CancellationToken ct = default);
        Task<CourseThumbnailDetails> GetThumbnailDetailsAsync(Guid courseId, CancellationToken ct = default);
        Task<PagedResult<CourseResponse>> GetAllAsync(GetCoursesQuery query, CourseStatus? status, CancellationToken ct = default);
        Task<CourseAccessData> GetCourseAccessData(Guid courseId, Guid instructorId, CancellationToken ct = default);
        Task<CourseCartData?> GetCourseCartDataAsync(Guid courseId, CancellationToken ct = default);
    }
    public sealed record CourseCreationData(bool CategoryExists, int SlugCount);
    public sealed record InstructorInfo(string FullName, string Email);
    public sealed record CourseThumbnailDetails(bool CourseExists, string? ThumbnailPublicId);
    public sealed record CourseAccessData(bool CourseExist, bool IsOwner);
    public sealed record CourseCartData(Guid Id, string Title, decimal Price, string? ThumbnailPublicId, bool IsPublished);

}
