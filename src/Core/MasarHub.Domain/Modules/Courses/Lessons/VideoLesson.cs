using MasarHub.Domain.SharedKernel;

namespace MasarHub.Domain.Modules.Courses.Lessons
{
    public sealed class VideoLesson : Lesson
    {
        public string VideoPublicId { get; private set; } = null!;
        public string? ThumbnailPublicId { get; private set; }
        public int DurationInSeconds { get; private set; }

        private VideoLesson() { }

        private VideoLesson(Guid moduleId, string title, int order, string? description, string videoPublicId, int duration)
            : base(moduleId, title, order, description)
        {
            VideoPublicId = Guard.AgainstNullOrWhiteSpace(videoPublicId, nameof(videoPublicId));
            DurationInSeconds = Guard.AgainstNegativeOrZero(duration, nameof(duration));
        }

        public static VideoLesson Create(Guid moduleId, string title, int order, string? description, string videoPublicId, int duration)
        {
            return new VideoLesson(moduleId, title, order, description, videoPublicId, duration);
        }
        public void UpdateThumbnail(string? thumbnailPublicId)
        {
            ThumbnailPublicId = thumbnailPublicId;
            MarkAsUpdated();
        }
    }
}
