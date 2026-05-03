using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

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
            ResourcePublicId = resourcePublicId;
            FileName = fileName;
            FileType = fileType;
            FileSizeInBytes = fileSize;
        }

        public static Result<ResourceLesson> Create(
            Guid moduleId,
            string title,
            int order,
            string? description,
            string publicId,
            string fileName,
            string fileType,
            long fileSize)
        {
            var error = GuardExtensions.FirstError(
                ValidateLesson(moduleId, title, order),
                Guard.AgainstNullOrWhiteSpace(publicId, nameof(publicId)),
                Guard.AgainstNullOrWhiteSpace(fileName, nameof(fileName)),
                Guard.AgainstNullOrWhiteSpace(fileType, nameof(fileType)),
                Guard.AgainstNegative(fileSize, nameof(fileSize))
            );

            if (error is not null)
                return error;

            return new ResourceLesson(moduleId, title, order, description, publicId, fileName, fileType, fileSize);
        }

        public Result UpdateFileName(string fileName)
        {
            var error = Guard.AgainstNullOrWhiteSpace(fileName, nameof(fileName));
            if (error is not null)
                return error;

            FileName = fileName;
            MarkAsUpdated();
            return Result.Success();
        }
    }
}
