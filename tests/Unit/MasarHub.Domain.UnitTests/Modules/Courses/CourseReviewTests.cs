using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Courses.Events;

namespace MasarHub.Domain.UnitTests.Modules.Courses
{
    [Trait("UnitTests.Domain.Courses", "CourseReview")]
    public sealed class CourseReviewTests
    {
        private static readonly Guid ValidUserId = Guid.NewGuid();
        private static readonly Guid ValidCourseId = Guid.NewGuid();

        #region Create

        [Fact]
        public void Create_ValidInput_ReturnsSuccess()
        {
            var result = CourseReview.Create(ValidUserId, ValidCourseId, 4.5, "Great course!");

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(ValidUserId, result.Value.UserId);
            Assert.Equal(ValidCourseId, result.Value.CourseId);
            Assert.Equal(4.5, result.Value.Rating);
            Assert.Equal("Great course!", result.Value.ReviewContent);
            Assert.False(result.Value.IsDeleted);
            Assert.Single(result.Value.DomainEvents);
            Assert.IsType<CourseReviewCreatedDomainEvent>(result.Value.DomainEvents.First());
        }

        [Fact]
        public void Create_ValidInputWithoutContent_ReturnsSuccess()
        {
            var result = CourseReview.Create(ValidUserId, ValidCourseId, 5.0);

            Assert.True(result.IsSuccess);
            Assert.Null(result.Value.ReviewContent);
        }

        [Fact]
        public void Create_EmptyUserId_ReturnsError()
        {
            var result = CourseReview.Create(Guid.Empty, ValidCourseId, 4.0);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public void Create_EmptyCourseId_ReturnsError()
        {
            var result = CourseReview.Create(ValidUserId, Guid.Empty, 4.0);

            Assert.True(result.IsFailure);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5.1)]
        [InlineData(10)]
        public void Create_InvalidRating_ReturnsError(double rating)
        {
            var result = CourseReview.Create(ValidUserId, ValidCourseId, rating);

            Assert.True(result.IsFailure);
            Assert.Equal("course_review.invalid_rating", result.Error.Code);
        }

        [Fact]
        public void Create_ValidLowerBoundaryRating_ReturnsSuccess()
        {
            var result = CourseReview.Create(ValidUserId, ValidCourseId, 1.0);

            Assert.True(result.IsSuccess);
            Assert.Equal(1.0, result.Value.Rating);
        }

        [Fact]
        public void Create_ValidUpperBoundaryRating_ReturnsSuccess()
        {
            var result = CourseReview.Create(ValidUserId, ValidCourseId, 5.0);

            Assert.True(result.IsSuccess);
            Assert.Equal(5.0, result.Value.Rating);
        }

        [Fact]
        public void Create_RaisesDomainEvent()
        {
            var result = CourseReview.Create(ValidUserId, ValidCourseId, 3.5, "Decent");

            Assert.True(result.IsSuccess);
            var domainEvent = Assert.Single(result.Value.DomainEvents);
            var createdEvent = Assert.IsType<CourseReviewCreatedDomainEvent>(domainEvent);
            Assert.Equal(result.Value.Id, createdEvent.ReviewId);
            Assert.Equal(ValidUserId, createdEvent.UserId);
            Assert.Equal(ValidCourseId, createdEvent.CourseId);
            Assert.Equal(3.5, createdEvent.Rating);
            Assert.Equal("Decent", createdEvent.ReviewContent);
        }

        #endregion

        #region UpdateRating

        [Fact]
        public void UpdateRating_ValidRating_ReturnsSuccess()
        {
            var review = CreateValidReview();

            var result = review.UpdateRating(2.0);

            Assert.True(result.IsSuccess);
            Assert.Equal(2.0, review.Rating);
            Assert.NotNull(review.EditedAt);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5.1)]
        [InlineData(10)]
        public void UpdateRating_InvalidRating_ReturnsError(double rating)
        {
            var review = CreateValidReview();

            var result = review.UpdateRating(rating);

            Assert.True(result.IsFailure);
            Assert.Equal("course_review.invalid_rating", result.Error.Code);
        }

        #endregion

        #region UpdateContent

        [Fact]
        public void UpdateContent_ValidContent_ReturnsSuccess()
        {
            var review = CreateValidReview();

            var result = review.UpdateContent("Updated review content");

            Assert.True(result.IsSuccess);
            Assert.Equal("Updated review content", review.ReviewContent);
            Assert.NotNull(review.EditedAt);
        }

        [Fact]
        public void UpdateContent_NullContent_ReturnsSuccess()
        {
            var review = CreateValidReview();
            review.UpdateContent("Some content");

            var result = review.UpdateContent(null);

            Assert.True(result.IsSuccess);
            Assert.Null(review.ReviewContent);
            Assert.NotNull(review.EditedAt);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_NotDeleted_ReturnsSuccess()
        {
            var review = CreateValidReview();

            var result = review.Delete();

            Assert.True(result.IsSuccess);
            Assert.True(review.IsDeleted);
            Assert.NotNull(review.DeletedAt);
        }

        [Fact]
        public void Delete_AlreadyDeleted_ReturnsError()
        {
            var review = CreateValidReview();
            review.Delete();

            var result = review.Delete();

            Assert.True(result.IsFailure);
        }

        #endregion

        private static CourseReview CreateValidReview()
        {
            return CourseReview.Create(ValidUserId, ValidCourseId, 4.0).Value;
        }
    }
}
