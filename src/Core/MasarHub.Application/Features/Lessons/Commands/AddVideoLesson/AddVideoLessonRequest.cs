namespace MasarHub.Application.Features.Lessons.Commands.AddVideoLesson
{
    public sealed record AddVideoLessonRequest
    (
        bool IsPreviewable,
        string Title,
        string? Description,
        string FileKey
    );
}
