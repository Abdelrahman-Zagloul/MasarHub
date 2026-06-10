using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Courses.Lessons
{
    public sealed class LessonAttachment : SoftDeletableEntity
    {
        public Guid LessonId { get; private set; }
        public string PublicId { get; private set; } = null!;
        public string FileName { get; private set; } = null!;
        public string FileType { get; private set; } = null!;
        public long FileSizeInBytes { get; private set; }

        private const int MaxAttachmentCount = 10;
        private LessonAttachment() { }

        private LessonAttachment(Guid lessonId, string publicId, string fileName, string fileType, long fileSizeInBytes)
        {
            LessonId = lessonId;
            PublicId = publicId;
            FileName = fileName;
            FileType = fileType;
            FileSizeInBytes = fileSizeInBytes;
        }

        public static DomainResult<LessonAttachment> Create(
            Guid lessonId,
            string publicId,
            string fileName,
            string fileType,
            long fileSizeInBytes)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(lessonId, nameof(lessonId)),
                Guard.AgainstNullOrWhiteSpace(publicId, nameof(publicId)),
                Guard.AgainstNullOrWhiteSpace(fileName, nameof(fileName)),
                Guard.AgainstNullOrWhiteSpace(fileType, nameof(fileType)),
                Guard.AgainstNegative(fileSizeInBytes, nameof(fileSizeInBytes))
            );

            if (error is not null)
                return error;

            return new LessonAttachment(lessonId, publicId, fileName, fileType, fileSizeInBytes);
        }

        public static DomainResult CanAddMoreAttachment(int currentAttachmentCount)
        {
            if (currentAttachmentCount > MaxAttachmentCount)
                return new DomainError("lesson.max_attachment_reached");

            return DomainResult.Success();
        }
        public DomainResult UpdateFileName(string fileName)
        {
            var error = Guard.AgainstNullOrWhiteSpace(fileName, nameof(fileName));
            if (error != DomainError.None)
                return error;

            FileName = fileName;
            MarkAsUpdated();
            return DomainResult.Success();
        }

        public DomainResult Delete() => MarkAsDeleted();
    }
}
