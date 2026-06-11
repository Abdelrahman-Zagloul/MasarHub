namespace MasarHub.Application.Features.Attachments.Commands.AddAttachment
{
    public sealed record AddAttachmentResponse
    (
        Guid AttachmentId,
        Guid LessonId,
        string AttachmentUrl,
        string AttachmentName,
        string AttachmentType,
        long FileSizeInByte
    );
}
