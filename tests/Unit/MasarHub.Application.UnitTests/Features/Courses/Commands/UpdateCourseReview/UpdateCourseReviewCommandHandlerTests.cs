using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Courses.Commands.UpdateCourseReview;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Courses.Commands.UpdateCourseReview
{
    [Trait("UnitTests.Feature.Courses", "UpdateCourseReview")]
    public sealed class UpdateCourseReviewCommandHandlerTests
    {
        private readonly Mock<IRepository<CourseReview>> _reviewRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateCourseReviewCommandHandler _sut;

        public UpdateCourseReviewCommandHandlerTests()
        {
            _reviewRepositoryMock = new Mock<IRepository<CourseReview>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new UpdateCourseReviewCommandHandler(
                _reviewRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        private static CourseReview CreateReview(Guid courseId, Guid userId, double rating, string? content)
        {
            var review = CourseReview.Create(userId, courseId, rating, content);
            return review.IsSuccess ? review.Value : throw new InvalidOperationException("Failed to create review");
        }

        [Fact]
        public async Task Handle_ReviewExistsAndIsOwner_UpdatesRating()
        {
            var courseId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateCourseReviewCommand(courseId, reviewId, userId, 3, null);
            var review = CreateReview(courseId, userId, 5, "Original content");

            _reviewRepositoryMock
                .Setup(x => x.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            review.Rating.Should().Be(3);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReviewExistsAndIsOwner_UpdatesContent()
        {
            var courseId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateCourseReviewCommand(courseId, reviewId, userId, null, "Updated content");
            var review = CreateReview(courseId, userId, 4, "Original content");

            _reviewRepositoryMock
                .Setup(x => x.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            review.ReviewContent.Should().Be("Updated content");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReviewExistsAndIsOwner_UpdatesBothRatingAndContent()
        {
            var courseId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateCourseReviewCommand(courseId, reviewId, userId, 2, "New content");
            var review = CreateReview(courseId, userId, 5, "Original content");

            _reviewRepositoryMock
                .Setup(x => x.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            review.Rating.Should().Be(2);
            review.ReviewContent.Should().Be("New content");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReviewNotFound_ReturnsNotFoundError()
        {
            var command = new UpdateCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 4, null);

            _reviewRepositoryMock
                .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseReview?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course_review.not_found");
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbiddenError()
        {
            var courseId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var command = new UpdateCourseReviewCommand(courseId, reviewId, otherUserId, 4, null);
            var review = CreateReview(courseId, Guid.NewGuid(), 5, null);

            _reviewRepositoryMock
                .Setup(x => x.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
        }

        [Fact]
        public async Task Handle_InvalidRating_ReturnsError()
        {
            var courseId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new UpdateCourseReviewCommand(courseId, reviewId, userId, 6, null);
            var review = CreateReview(courseId, userId, 4, null);

            _reviewRepositoryMock
                .Setup(x => x.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course_review.invalid_rating");
        }
    }
}
