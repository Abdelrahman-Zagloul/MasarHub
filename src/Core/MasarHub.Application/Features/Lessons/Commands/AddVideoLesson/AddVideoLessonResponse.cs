namespace MasarHub.Application.Features.Lessons.Commands.AddVideoLesson
{
    public sealed record AddVideoLessonResponse
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
