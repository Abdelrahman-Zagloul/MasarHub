using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Reviews.Commands.CreateCourseReview;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Reviews.Commands.CreateCourseReview
{
    [Trait("UnitTests.Feature.Courses", "CreateCourseReview")]
    public sealed class CreateCourseReviewCommandHandlerTests
    {
        private readonly Mock<ICourseReviewQuery> _courseReviewQueryMock;
        private readonly Mock<IRepository<CourseReview>> _reviewRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateCourseReviewCommandHandler _sut;

        public CreateCourseReviewCommandHandlerTests()
        {
            _courseReviewQueryMock = new Mock<ICourseReviewQuery>();
            _reviewRepositoryMock = new Mock<IRepository<CourseReview>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new CreateCourseReviewCommandHandler(
                _courseReviewQueryMock.Object,
                _reviewRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_EnrolledAndNoExistingReview_ReturnsSuccess()
        {
            var courseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new CreateCourseReviewCommand(courseId, userId, 5, "Great course!");

            _courseReviewQueryMock
                .Setup(x => x.GetCreateReviewCheckAsync(courseId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateReviewCheckResult(true, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Rating.Should().Be(5);
            result.Value.ReviewContent.Should().Be("Great course!");
            _reviewRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CourseReview>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NotEnrolled_ReturnsForbiddenError()
        {
            var command = new CreateCourseReviewCommand(Guid.NewGuid(), Guid.NewGuid(), 4, null);

            _courseReviewQueryMock
                .Setup(x => x.GetCreateReviewCheckAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateReviewCheckResult(false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_enrolled");
        }

        [Fact]
        public async Task Handle_AlreadyReviewed_ReturnsConflictError()
        {
            var courseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new CreateCourseReviewCommand(courseId, userId, 3, null);

            _courseReviewQueryMock
                .Setup(x => x.GetCreateReviewCheckAsync(courseId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateReviewCheckResult(true, true));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course_review.already_reviewed");
        }

        [Fact]
        public async Task Handle_InvalidRating_ReturnsError()
        {
            var courseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new CreateCourseReviewCommand(courseId, userId, 6, null);

            _courseReviewQueryMock
                .Setup(x => x.GetCreateReviewCheckAsync(courseId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateReviewCheckResult(true, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course_review.invalid_rating");
        }
    }
}
