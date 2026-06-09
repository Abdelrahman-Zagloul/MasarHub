namespace MasarHub.Application.Features.Lessons.Commands.CreateArticleLesson
{
    public record CreateArticleLessonResponse
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
