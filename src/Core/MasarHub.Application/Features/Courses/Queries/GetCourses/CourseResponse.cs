namespace MasarHub.Application.Features.Courses.Queries.GetCourses
{
    public sealed record CourseResponse
    (
         Guid Id,
         string Title,
         string Slug,
         decimal Price,
         string Language,
         string Status,
         string Level,
         DateTimeOffset? PublishedAt,
         Guid InstructorId,
         string InstructorName,
         Guid CategoryId,
         string CategoryName
    );
}
