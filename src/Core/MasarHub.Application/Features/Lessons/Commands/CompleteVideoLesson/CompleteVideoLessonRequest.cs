namespace MasarHub.Application.Features.Lessons.Commands.CompleteVideoLesson
{
    public sealed record CompleteVideoLessonRequest
    (
        bool IsPreviewable,
        string Title,
        string? Description,
        string FileKey
    );
}
