using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Courses.Lessons
{
    public sealed class VideoLesson : Lesson
    {
        public string VideoPublicId { get; private set; } = null!;
        public string? ThumbnailPublicId { get; private set; }
        public int DurationInSeconds { get; private set; }

        private VideoLesson() { }

        private VideoLesson(Guid moduleId, bool isPreviewable, string title, int order, string? description, string videoPublicId, int duration)
            : base(moduleId, isPreviewable, title, order, description)
        {
            VideoPublicId = videoPublicId;
            DurationInSeconds = duration;
        }

        public static DomainResult<VideoLesson> Create(Guid moduleId, bool isPreviewable, string title, int order, string? description, string videoPublicId, int duration)
        {
            var error = GuardExtensions.FirstError(
                ValidateLesson(moduleId, title, order),
                Guard.AgainstNullOrWhiteSpace(videoPublicId, nameof(videoPublicId)),
                Guard.AgainstNegativeOrZero(duration, nameof(duration))
            );

            if (error is not null)
                return error;

            return new VideoLesson(moduleId, isPreviewable, title, order, description, videoPublicId, duration);
        }

        public DomainResult UpdateThumbnail(string? thumbnailPublicId)
        {
            ThumbnailPublicId = thumbnailPublicId;
            MarkAsUpdated();
            return DomainResult.Success();
        }
    }
}
