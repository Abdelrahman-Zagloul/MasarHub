using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Courses.Events;

namespace MasarHub.Domain.Modules.Courses.Lessons
{
    public sealed class VideoLesson : Lesson
    {
        public string VideoPublicId { get; private set; } = null!;
        public string? ThumbnailPublicId { get; private set; }
        public string FileName { get; private set; } = null!;
        public long FileSizeInByte { get; private set; }
        public double DurationInSeconds { get; private set; }

        private VideoLesson() { }

        private VideoLesson(Guid moduleId, bool isPreviewable, string title, int order, string? description, string videoPublicId, string fileName, long fileSizeInByte, double durationInSeconds)
            : base(moduleId, isPreviewable, title, order, description)
        {
            VideoPublicId = videoPublicId;
            FileName = fileName;
            FileSizeInByte = fileSizeInByte;
            DurationInSeconds = durationInSeconds;
        }

        public static DomainResult<VideoLesson> Create(Guid moduleId, bool isPreviewable, string title, int order, string? description, string videoPublicId, string fileName, long fileSizeInByte, double durationInSeconds)
        {
            var error = GuardExtensions.FirstError(
                ValidateLesson(moduleId, title, order),
                Guard.AgainstNullOrWhiteSpace(videoPublicId, nameof(videoPublicId)),
                Guard.AgainstNullOrWhiteSpace(fileName, nameof(fileName)),
                Guard.AgainstNegativeOrZero(fileSizeInByte, nameof(fileSizeInByte)),
                Guard.AgainstNegativeOrZero(durationInSeconds, nameof(durationInSeconds))
            );

            if (error is not null)
                return error;

            return new VideoLesson(moduleId, isPreviewable, title, order, description, videoPublicId, fileName, fileSizeInByte, durationInSeconds);
        }

        public DomainResult UpdateThumbnail(string? thumbnailPublicId)
        {
            var oldThumbnail = ThumbnailPublicId;
            ThumbnailPublicId = thumbnailPublicId;
            MarkAsUpdated();

            if (!string.IsNullOrWhiteSpace(oldThumbnail))
                RaiseDomainEvent(new ThumbnailChangedDomainEvent(oldThumbnail));

            return DomainResult.Success();
        }
    }
}
