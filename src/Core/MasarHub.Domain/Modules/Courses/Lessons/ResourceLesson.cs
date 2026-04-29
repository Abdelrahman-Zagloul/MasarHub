using MasarHub.Domain.SharedKernel;

namespace MasarHub.Domain.Modules.Courses.Lessons
{
    public sealed class ResourceLesson : Lesson
    {
        public string ResourcePublicId { get; private set; } = null!;
        public string FileName { get; private set; } = null!;
        public string FileType { get; private set; } = null!;
        public long FileSizeInBytes { get; private set; }

        private ResourceLesson() { }

        private ResourceLesson(
            Guid moduleId,
            string title,
            int order,
            string? description,
            string resourcePublicId,
            string fileName,
            string fileType,
            long fileSize)
            : base(moduleId, title, order, description)
        {
            ResourcePublicId = Guard.AgainstNullOrWhiteSpace(resourcePublicId, nameof(resourcePublicId));
            FileName = Guard.AgainstNullOrWhiteSpace(fileName, nameof(fileName));
            FileType = Guard.AgainstNullOrWhiteSpace(fileType, nameof(fileType));
            FileSizeInBytes = Guard.AgainstNegative(fileSize, nameof(fileSize));
        }

        public static ResourceLesson Create(
            Guid moduleId,
            string title,
            int order,
            string? description,
            string publicId,
            string fileName,
            string fileType,
            long fileSize)
        {
            return new ResourceLesson(moduleId, title, order, description, publicId, fileName, fileType, fileSize);
        }

        public void UpdateFileName(string fileName)
        {
            FileName = Guard.AgainstNullOrWhiteSpace(fileName, nameof(fileName));
            MarkAsUpdated();
        }
    }
}