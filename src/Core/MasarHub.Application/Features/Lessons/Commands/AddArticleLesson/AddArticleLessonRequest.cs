namespace MasarHub.Application.Features.Lessons.Commands.CreateArticleLesson
{
    public sealed record AddArticleLessonRequest
    (
        bool IsPreviewable,
        string Title,
        string Content,
        string? Description
    );
}
