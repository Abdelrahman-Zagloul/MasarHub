using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.CreateArticleLesson
{
    public sealed record CreateArticleLessonCommand
    (
        Guid CourseId,
        Guid ModuleId,
        Guid InstructorId,
        bool IsPreviewable,
        string Title,
        string Content,
        string? Description
    ) : IRequest<Result<CreateArticleLessonResponse>>;
}
