using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Features.Courses.Queries.GetCourseById;

namespace MasarHub.Application.Abstractions.Persistence.Queries
{
    public interface ICourseQuery : IScopedService
    {
        Task<(bool CategoryExists, int SlugCount)> GetCreationDataAsync(string slug, Guid categoryId, CancellationToken ct = default);
        Task<(string FullName, string Email)> GetInstructorInfoAsync(Guid instructorId, CancellationToken ct = default);
        Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken ct = default);
        Task<bool> HasLecturesAsync(Guid courseId, CancellationToken cancellationToken);
        Task<CourseDetailsResponse?> GetDetailsByIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<(bool CourseExists, string? ThumbnailPublicId)> GetThumbnailDetailsAsync(Guid courseId, CancellationToken cancellationToken);
    }
}
