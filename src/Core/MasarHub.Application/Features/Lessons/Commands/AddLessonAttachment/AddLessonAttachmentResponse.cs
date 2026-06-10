namespace MasarHub.Application.Features.Lessons.Commands.AddLessonAttachment
{
    public sealed record AddLessonAttachmentResponse
    (
        Guid AttachmentId,
        Guid CourseId,
        Guid ModuleId,
        Guid LessonId,
        string AttachmentUrl,
        string AttachmentName,
        string AttachmentType,
        long FileSizeInByte
    );
}
