using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Application.Features.Courses.Commands.CreateCourse
{
    public sealed record CreateCourseResponse
    (
        Guid Id,
        string Title,
        string Slug,
        decimal Price,
        CourseStatus Status,
        Guid InstructorId,
        Guid CategoryId
    );
}