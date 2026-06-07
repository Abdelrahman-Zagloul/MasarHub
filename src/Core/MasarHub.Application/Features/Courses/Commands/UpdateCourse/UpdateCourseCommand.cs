using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.UpdateCourse
{
    public sealed record UpdateCourseCommand
    (
        Guid CourseId,
        string? Title,
        string? Description,
        decimal? Price,
        CourseLanguage? Language,
        CourseLevel? Level,
        Guid? CategoryId
    ) : IRequest<Result>;
}
