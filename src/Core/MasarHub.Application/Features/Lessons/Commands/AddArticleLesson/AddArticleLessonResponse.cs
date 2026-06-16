namespace MasarHub.Application.Features.Lessons.Commands.AddArticleLesson
{
    public record AddArticleLessonResponse
    (
        Guid Id,
        Guid ModuleId,
        bool IsPreviewable,
        int DisplayOrder,
        string Title,
        string Content,
        string? Description
    );
}
