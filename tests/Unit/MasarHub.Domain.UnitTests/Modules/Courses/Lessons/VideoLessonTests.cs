using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Courses.Events;
using MasarHub.Domain.Modules.Courses.Lessons;

namespace MasarHub.Domain.UnitTests.Modules.Courses.Lessons
{
    public sealed class VideoLessonTests
    {
        private static readonly Guid ModuleId = Guid.NewGuid();
        private const string Title = "Advanced LINQ";
        private const int DisplayOrder = 2;
        private const string VideoPublicId = "video_abc123";
        private const string FileName = "advanced-linq.mp4";
        private const long FileSizeInByte = 50_000_000;
        private const double DurationInSeconds = 1200;

        #region Create

        [Fact]
        public void Create_ValidInput_ReturnsSuccess()
        {
            var result = VideoLesson.Create(ModuleId, true, Title, DisplayOrder, null, VideoPublicId, FileName, FileSizeInByte, DurationInSeconds);

            Assert.True(result.IsSuccess);
            Assert.Equal(ModuleId, result.Value.ModuleId);
            Assert.Equal(Title, result.Value.Title);
            Assert.True(result.Value.IsPreviewable);
            Assert.Equal(VideoPublicId, result.Value.VideoPublicId);
            Assert.Equal(FileName, result.Value.FileName);
            Assert.Equal(FileSizeInByte, result.Value.FileSizeInByte);
            Assert.Equal(DurationInSeconds, result.Value.DurationInSeconds);
            Assert.Equal(LessonStatus.Active, result.Value.LessonStatus);
        }

        [Fact]
        public void Create_WithDescription_ReturnsSuccess()
        {
            var description = "Learn advanced LINQ techniques";

            var result = VideoLesson.Create(ModuleId, false, Title, DisplayOrder, description, VideoPublicId, FileName, FileSizeInByte, DurationInSeconds);

            Assert.True(result.IsSuccess);
            Assert.Equal(description, result.Value.Description);
            Assert.False(result.Value.IsPreviewable);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidTitle_ReturnsError(string? title)
        {
            var result = VideoLesson.Create(ModuleId, true, title!, DisplayOrder, null, VideoPublicId, FileName, FileSizeInByte, DurationInSeconds);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Create_EmptyModuleId_ReturnsError()
        {
            var result = VideoLesson.Create(Guid.Empty, true, Title, DisplayOrder, null, VideoPublicId, FileName, FileSizeInByte, DurationInSeconds);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("moduleId").Code, result.Error.Code);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_InvalidDisplayOrder_ReturnsError(int order)
        {
            var result = VideoLesson.Create(ModuleId, true, Title, order, null, VideoPublicId, FileName, FileSizeInByte, DurationInSeconds);

            Assert.True(result.IsFailure);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidVideoPublicId_ReturnsError(string? publicId)
        {
            var result = VideoLesson.Create(ModuleId, true, Title, DisplayOrder, null, publicId!, FileName, FileSizeInByte, DurationInSeconds);

            Assert.True(result.IsFailure);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidFileName_ReturnsError(string? fileName)
        {
            var result = VideoLesson.Create(ModuleId, true, Title, DisplayOrder, null, VideoPublicId, fileName!, FileSizeInByte, DurationInSeconds);

            Assert.True(result.IsFailure);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_InvalidFileSize_ReturnsError(long fileSize)
        {
            var result = VideoLesson.Create(ModuleId, true, Title, DisplayOrder, null, VideoPublicId, FileName, fileSize, DurationInSeconds);

            Assert.True(result.IsFailure);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_InvalidDuration_ReturnsError(double duration)
        {
            var result = VideoLesson.Create(ModuleId, true, Title, DisplayOrder, null, VideoPublicId, FileName, FileSizeInByte, duration);

            Assert.True(result.IsFailure);
        }

        #endregion

        #region UpdateThumbnail

        [Fact]
        public void UpdateThumbnail_NoPreviousThumbnail_ReturnsSuccess()
        {
            var lesson = CreateValidLesson();
            var newThumbnail = "thumb_new.jpg";

            var result = lesson.UpdateThumbnail(newThumbnail);

            Assert.True(result.IsSuccess);
            Assert.Equal(newThumbnail, lesson.ThumbnailPublicId);
            Assert.Empty(lesson.DomainEvents);
        }

        [Fact]
        public void UpdateThumbnail_WithPreviousThumbnail_RaisesDomainEvent()
        {
            var lesson = CreateValidLessonWithThumbnail();
            var newThumbnail = "thumb_new.jpg";

            var result = lesson.UpdateThumbnail(newThumbnail);

            Assert.True(result.IsSuccess);
            Assert.Equal(newThumbnail, lesson.ThumbnailPublicId);
            Assert.Contains(lesson.DomainEvents, e => e is ThumbnailChangedDomainEvent);
        }

        [Fact]
        public void UpdateThumbnail_SetToNull_ReturnsSuccess()
        {
            var lesson = CreateValidLessonWithThumbnail();

            var result = lesson.UpdateThumbnail(null);

            Assert.True(result.IsSuccess);
            Assert.Null(lesson.ThumbnailPublicId);
            Assert.Contains(lesson.DomainEvents, e => e is ThumbnailChangedDomainEvent);
        }

        #endregion

        private static VideoLesson CreateValidLesson()
        {
            return VideoLesson.Create(ModuleId, true, Title, DisplayOrder, null, VideoPublicId, FileName, FileSizeInByte, DurationInSeconds).Value;
        }

        private static VideoLesson CreateValidLessonWithThumbnail()
        {
            var lesson = CreateValidLesson();
            lesson.UpdateThumbnail("thumb_old.jpg");
            lesson.ClearDomainEvents();
            return lesson;
        }
    }
}
