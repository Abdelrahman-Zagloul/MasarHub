namespace MasarHub.Application.Features.Lessons.Commands.CreateArticleLesson
{
    public sealed record CreateArticleLessonRequest
    (
        bool IsPreviewable,
        string Title,
        string Content,
        string? Description
    );
}
