using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;

namespace MasarHub.Domain.Modules.Courses.Lessons
{
    public sealed class LessonAttachment : SoftDeletableEntity
    {
        public Guid LessonId { get; private set; }

        public string PublicId { get; private set; } = null!;
        public string FileName { get; private set; } = null!;
        public string FileType { get; private set; } = null!;
        public long FileSizeInBytes { get; private set; }

        private LessonAttachment() { }

        private LessonAttachment(
            Guid lessonId,
            string publicId,
            string fileName,
            string fileType,
            long fileSizeInBytes)
        {
            LessonId = Guard.AgainstEmptyGuid(lessonId, nameof(lessonId));
            PublicId = Guard.AgainstNullOrWhiteSpace(publicId, nameof(publicId));
            FileName = Guard.AgainstNullOrWhiteSpace(fileName, nameof(fileName));
            FileType = Guard.AgainstNullOrWhiteSpace(fileType, nameof(fileType));
            FileSizeInBytes = Guard.AgainstNegative(fileSizeInBytes, nameof(fileSizeInBytes));
        }

        public static LessonAttachment Create(
            Guid lessonId,
            string publicId,
            string fileName,
            string fileType,
            long fileSizeInBytes)
        {
            return new LessonAttachment(lessonId, publicId, fileName, fileType, fileSizeInBytes);
        }

        public void UpdateFileName(string fileName)
        {
            FileName = Guard.AgainstNullOrWhiteSpace(fileName, nameof(fileName));
            MarkAsUpdated();
        }
    }
}