using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Courses;
using MediatR;

namespace MasarHub.Application.Features.Courses.Commands.CreateCourse
{
    public sealed record CreateCourseCommand
    (
        string Title,
        string Description,
        decimal Price,
        CourseLanguage Language,
        CourseLevel Level,
        Guid CategoryId
    ) : IRequest<Result<CreateCourseResponse>>;
}
