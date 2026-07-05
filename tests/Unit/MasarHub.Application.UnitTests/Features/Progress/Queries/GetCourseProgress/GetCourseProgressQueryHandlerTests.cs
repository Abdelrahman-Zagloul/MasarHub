using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Progress.Queries.GetCourseProgress;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Progress.Queries.GetCourseProgress
{
    [Trait("UnitTests.Feature.Progress", "GetCourseProgress")]
    public sealed class GetCourseProgressQueryHandlerTests
    {
        private readonly Mock<IProgressQuery> _progressQueryMock;
        private readonly GetCourseProgressQueryHandler _sut;

        public GetCourseProgressQueryHandlerTests()
        {
            _progressQueryMock = new Mock<IProgressQuery>();
            _sut = new GetCourseProgressQueryHandler(_progressQueryMock.Object);
        }

        [Fact]
        public async Task Handle_NotEnrolled_ReturnsForbiddenError()
        {
            var query = new GetCourseProgressQuery(Guid.NewGuid(), Guid.NewGuid());

            _progressQueryMock
                .Setup(x => x.GetCourseProgressAsync(query.UserId, query.CourseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseProgressData(false, null, []));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_enrolled");
        }

        [Fact]
        public async Task Handle_EnrolledNoProgress_ReturnsDefaultResponse()
        {
            var courseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var query = new GetCourseProgressQuery(userId, courseId);

            _progressQueryMock
                .Setup(x => x.GetCourseProgressAsync(userId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseProgressData(true, null, []));

            _progressQueryMock
                .Setup(x => x.GetCourseLessonCountAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(10);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.CourseId.Should().Be(courseId);
            result.Value.CompletedLessons.Should().Be(0);
            result.Value.TotalLessons.Should().Be(10);
            result.Value.IsCompleted.Should().BeFalse();
            result.Value.CompletedAt.Should().BeNull();
            result.Value.Modules.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_EnrolledWithProgress_ReturnsFullResponse()
        {
            var courseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var query = new GetCourseProgressQuery(userId, courseId);
            var completedAt = DateTimeOffset.UtcNow;
            var courseProgress = CourseProgress.Create(userId, courseId, 5).Value;
            var modules = new List<ModuleProgressItem>
            {
                new(Guid.NewGuid(), "Module 1", 1, 2, 3),
                new(Guid.NewGuid(), "Module 2", 2, 3, 3)
            };

            _progressQueryMock
                .Setup(x => x.GetCourseProgressAsync(userId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseProgressData(true, courseProgress, modules));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.CourseId.Should().Be(courseId);
            result.Value.CompletedLessons.Should().Be(0);
            result.Value.TotalLessons.Should().Be(5);
            result.Value.IsCompleted.Should().BeFalse();
            result.Value.CompletedAt.Should().BeNull();
            result.Value.Modules.Should().BeEquivalentTo(modules);
        }

        [Fact]
        public async Task Handle_EnrolledWithProgressCompleted_ReturnsIsCompletedTrue()
        {
            var courseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var query = new GetCourseProgressQuery(userId, courseId);

            var courseProgress = CourseProgress.Create(userId, courseId, 3).Value;
            courseProgress.MarkLessonCompleted();
            courseProgress.MarkLessonCompleted();
            courseProgress.MarkLessonCompleted();

            var modules = new List<ModuleProgressItem>
            {
                new(Guid.NewGuid(), "Module 1", 1, 3, 3)
            };

            _progressQueryMock
                .Setup(x => x.GetCourseProgressAsync(userId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseProgressData(true, courseProgress, modules));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.CompletedLessons.Should().Be(3);
            result.Value.TotalLessons.Should().Be(3);
            result.Value.IsCompleted.Should().BeTrue();
            result.Value.CompletedAt.Should().NotBeNull();
        }
    }
}
