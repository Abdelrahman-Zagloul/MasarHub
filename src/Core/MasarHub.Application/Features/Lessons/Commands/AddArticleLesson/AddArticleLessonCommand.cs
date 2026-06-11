using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Lessons.Commands.CreateArticleLesson;
using MediatR;

namespace MasarHub.Application.Features.Lessons.Commands.AddArticleLesson
{
    public sealed record AddArticleLessonCommand
    (
        Guid ModuleId,
        Guid InstructorId,
        bool IsPreviewable,
        string Title,
        string Content,
        string? Description
    ) : IRequest<Result<AddArticleLessonResponse>>;
}
