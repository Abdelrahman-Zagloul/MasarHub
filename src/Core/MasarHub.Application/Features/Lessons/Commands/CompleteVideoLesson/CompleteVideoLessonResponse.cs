namespace MasarHub.Application.Features.Lessons.Commands.CompleteVideoLesson
{
    public sealed record CompleteVideoLessonResponse
    (
        Guid Id,
        Guid ModuleId,
        bool IsPreviewable,
        int DisplayOrder,
        string Title,
        string? Description,
        string VideoUrl,
        string FileName,
        long FileSizeInByte,
        double DurationInSeconds
    );
}
