using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCourse
{
    public sealed record UpdateCourseRequest
    (
        string? Title,
        string? Description,
        decimal? Price,
        CourseLanguage? Language,
        CourseLevel? Level,
        Guid? CategoryId
    );
}
