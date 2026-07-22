using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Reviews.Commands.DeleteCourseReview;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Reviews.Commands.DeleteCourseReview
{
    [Trait("UnitTests.Feature.Courses", "DeleteCourseReview")]
    public sealed class DeleteCourseReviewCommandHandlerTests
    {
        private readonly Mock<IRepository<CourseReview>> _reviewRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeleteCourseReviewCommandHandler _sut;

        public DeleteCourseReviewCommandHandlerTests()
        {
            _reviewRepositoryMock = new Mock<IRepository<CourseReview>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new DeleteCourseReviewCommandHandler(
                _reviewRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ReviewExistsAndIsOwner_DeletesSuccessfully()
        {
            var courseId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new DeleteCourseReviewCommand(courseId, reviewId, userId);
            var review = CourseReview.Create(userId, courseId, 4, null).Value;

            _reviewRepositoryMock
                .Setup(x => x.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            review.IsDeleted.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReviewNotFound_ReturnsNotFoundError()
        {
            var command = new DeleteCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

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
            var command = new DeleteCourseReviewCommand(courseId, reviewId, otherUserId);
            var review = CourseReview.Create(Guid.NewGuid(), courseId, 4, null).Value;

            _reviewRepositoryMock
                .Setup(x => x.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
        }
    }
}
