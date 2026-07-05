using FluentAssertions;
using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Application.Features.Progress.Commands.CompleteLesson;
using MasarHub.Domain.Modules.Courses.Events;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Progress.Commands.CompleteLesson
{
    [Trait("UnitTests.Feature.Progress", "LessonCompletedEvent")]
    public sealed class LessonCompletedEventHandlerTests
    {
        private readonly Mock<IBackgroundJobService> _backgroundJobServiceMock;
        private readonly LessonCompletedEventHandler _sut;

        public LessonCompletedEventHandlerTests()
        {
            _backgroundJobServiceMock = new Mock<IBackgroundJobService>();
            _sut = new LessonCompletedEventHandler(_backgroundJobServiceMock.Object);
        }

        [Fact]
        public async Task Handle_EnqueuesProgressUpdateJob()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var moduleId = Guid.NewGuid();
            var lessonId = Guid.NewGuid();
            var domainEvent = new LessonProgressCreatedDomainEvent(userId, courseId, moduleId, lessonId);
            var notification = new DomainEventNotification<LessonProgressCreatedDomainEvent>(domainEvent);

            await _sut.Handle(notification, CancellationToken.None);

            _backgroundJobServiceMock.Verify(
                x => x.Enqueue<IProgressJob>(It.IsAny<System.Linq.Expressions.Expression<System.Func<IProgressJob, Task>>>()),
                Times.Once);
        }
    }
}
