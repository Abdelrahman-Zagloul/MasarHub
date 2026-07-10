using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Features.Courses.Queries.GetCourseById
{
    public sealed record CourseDetailsResponse
    (
        Guid Id,
        string Title,
        string Slug,
        string Description,
        decimal Price,
        CourseLanguage Language,
        CourseStatus Status,
        CourseLevel Level,
        DateTimeOffset? PublishedAt,
        Guid InstructorId,
        string InstructorName,
        Guid CategoryId,
        string CategoryName,
        string? ThumbnailPublicId,
        string? RejectionReason
    )
    {
        public string? ThumbnailUrl { get; set; }
        public List<string> Prerequisites { get; set; } = [];
        public List<string> Requirements { get; set; } = [];
        public List<string> LearningObjectives { get; set; } = [];
        public List<ModuleResponse> Modules { get; set; } = [];
    }

    public sealed record ModuleResponse
    (
        Guid ModuleId,
        string Title,
        string? Description,
        int DisplayOrder,
        int LessonCount
    );
}
