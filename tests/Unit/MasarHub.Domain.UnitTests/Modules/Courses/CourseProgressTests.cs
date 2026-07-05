using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Modules.Courses;

namespace MasarHub.Domain.UnitTests.Modules.Courses
{
    [Trait("UnitTests.Domain.Courses", "CourseProgress")]
    public sealed class CourseProgressTests
    {
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly Guid CourseId = Guid.NewGuid();
        private const int TotalLessons = 5;

        #region Create

        [Fact]
        public void Create_ValidInput_ReturnsSuccess()
        {
            var result = CourseProgress.Create(UserId, CourseId, TotalLessons);

            Assert.True(result.IsSuccess);
            Assert.Equal(UserId, result.Value.UserId);
            Assert.Equal(CourseId, result.Value.CourseId);
            Assert.Equal(TotalLessons, result.Value.TotalLessons);
            Assert.Equal(0, result.Value.CompletedLessons);
            Assert.Null(result.Value.CompletedAt);
            Assert.Empty(result.Value.DomainEvents);
        }

        [Fact]
        public void Create_EmptyUserId_ReturnsError()
        {
            var result = CourseProgress.Create(Guid.Empty, CourseId, TotalLessons);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("userId").Code, result.Error.Code);
        }

        [Fact]
        public void Create_EmptyCourseId_ReturnsError()
        {
            var result = CourseProgress.Create(UserId, Guid.Empty, TotalLessons);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.EmptyGuid("courseId").Code, result.Error.Code);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Create_InvalidTotalLessons_ReturnsError(int totalLessons)
        {
            var result = CourseProgress.Create(UserId, CourseId, totalLessons);

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.NegativeOrZero("totalLessons").Code, result.Error.Code);
        }

        #endregion

        #region MarkLessonCompleted

        [Fact]
        public void MarkLessonCompleted_IncrementsCompletedLessons()
        {
            var courseProgress = CreateValidCourseProgress();

            courseProgress.MarkLessonCompleted();

            Assert.Equal(1, courseProgress.CompletedLessons);
            Assert.Null(courseProgress.CompletedAt);
        }

        [Fact]
        public void MarkLessonCompleted_AllLessonsCompleted_SetsCompletedAt()
        {
            var courseProgress = CreateValidCourseProgress();

            for (int i = 0; i < TotalLessons; i++)
                courseProgress.MarkLessonCompleted();

            Assert.Equal(TotalLessons, courseProgress.CompletedLessons);
            Assert.NotNull(courseProgress.CompletedAt);
        }

        [Fact]
        public void MarkLessonCompleted_AlreadyCompleted_DoesNotIncrement()
        {
            var courseProgress = CreateValidCourseProgress();
            for (int i = 0; i < TotalLessons; i++)
                courseProgress.MarkLessonCompleted();

            var completedAt = courseProgress.CompletedAt;

            courseProgress.MarkLessonCompleted();

            Assert.Equal(TotalLessons, courseProgress.CompletedLessons);
            Assert.Equal(completedAt, courseProgress.CompletedAt);
        }

        #endregion

        #region UpdateTotalLessons

        [Fact]
        public void UpdateTotalLessons_ValidInput_ReturnsSuccess()
        {
            var courseProgress = CreateValidCourseProgress();
            var newTotal = 10;

            var result = courseProgress.UpdateTotalLessons(newTotal);

            Assert.True(result.IsSuccess);
            Assert.Equal(newTotal, courseProgress.TotalLessons);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void UpdateTotalLessons_InvalidInput_ReturnsError(int totalLessons)
        {
            var courseProgress = CreateValidCourseProgress();

            var result = courseProgress.UpdateTotalLessons(totalLessons);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void UpdateTotalLessons_LessThanCompleted_ClampsCompletedLessons()
        {
            var courseProgress = CreateValidCourseProgress();
            courseProgress.MarkLessonCompleted();
            courseProgress.MarkLessonCompleted();
            courseProgress.MarkLessonCompleted();

            var result = courseProgress.UpdateTotalLessons(2);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, courseProgress.TotalLessons);
            Assert.Equal(2, courseProgress.CompletedLessons);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_ReturnsSuccess()
        {
            var courseProgress = CreateValidCourseProgress();

            var result = courseProgress.Delete();

            Assert.True(result.IsSuccess);
            Assert.True(courseProgress.IsDeleted);
            Assert.NotNull(courseProgress.DeletedAt);
        }

        #endregion

        private static CourseProgress CreateValidCourseProgress()
        {
            return CourseProgress.Create(UserId, CourseId, TotalLessons).Value;
        }
    }
}
