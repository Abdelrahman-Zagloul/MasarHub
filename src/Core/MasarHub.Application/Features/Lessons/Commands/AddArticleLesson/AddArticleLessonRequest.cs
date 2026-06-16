namespace MasarHub.Application.Features.Lessons.Commands.AddArticleLesson
{
    public sealed record AddArticleLessonRequest
    (
        bool IsPreviewable,
        string Title,
        string Content,
        string? Description
    );
}
