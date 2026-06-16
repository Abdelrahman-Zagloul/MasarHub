using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Lessons.Commands.ReorderLessons;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Lessons.Commands.ReorderLessons
{
    [Trait("UnitTests.Feature.Lessons", "ReorderLessons")]
    public sealed class ReorderLessonsCommandHandlerTests
    {
        private readonly Mock<ILessonQuery> _lessonQueryMock;
        private readonly ReorderLessonsCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public ReorderLessonsCommandHandlerTests()
        {
            _lessonQueryMock = new Mock<ILessonQuery>();
            _sut = new ReorderLessonsCommandHandler(_lessonQueryMock.Object);
        }

        [Fact]
        public async Task Handle_ModuleNotFound_ReturnsNotFoundError()
        {
            var command = new ReorderLessonsCommand(Guid.NewGuid(), InstructorId, [Guid.NewGuid()]);

            _lessonQueryMock
                .Setup(x => x.GetModuleAccessDataAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleAccessData(false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "module.not_found");
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbiddenError()
        {
            var command = new ReorderLessonsCommand(Guid.NewGuid(), InstructorId, [Guid.NewGuid()]);

            _lessonQueryMock
                .Setup(x => x.GetModuleAccessDataAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleAccessData(true, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
        }

        [Fact]
        public async Task Handle_CountMismatch_ReturnsBadRequestError()
        {
            var command = new ReorderLessonsCommand(Guid.NewGuid(), InstructorId, [Guid.NewGuid(), Guid.NewGuid()]);

            _lessonQueryMock
                .Setup(x => x.GetModuleAccessDataAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleAccessData(true, true));
            _lessonQueryMock
                .Setup(x => x.GetLessonIdsByModuleIdAsync(command.ModuleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([Guid.NewGuid()]);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "lesson.reorder_items_mismatch");
        }

        [Fact]
        public async Task Handle_LessonNotFoundInModule_ReturnsBadRequestError()
        {
            var differentId = Guid.NewGuid();
            var command = new ReorderLessonsCommand(Guid.NewGuid(), InstructorId, [differentId]);

            _lessonQueryMock
                .Setup(x => x.GetModuleAccessDataAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleAccessData(true, true));
            _lessonQueryMock
                .Setup(x => x.GetLessonIdsByModuleIdAsync(command.ModuleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([Guid.NewGuid()]);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "lesson.reorder_lesson_not_found");
        }

        [Fact]
        public async Task Handle_BulkUpdateFails_ReturnsFailure()
        {
            var lessonId = Guid.NewGuid();
            var command = new ReorderLessonsCommand(Guid.NewGuid(), InstructorId, [lessonId]);

            _lessonQueryMock
                .Setup(x => x.GetModuleAccessDataAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleAccessData(true, true));
            _lessonQueryMock
                .Setup(x => x.GetLessonIdsByModuleIdAsync(command.ModuleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([lessonId]);
            _lessonQueryMock
                .Setup(x => x.BulkUpdateDisplayOrderAsync(command.ModuleId, command.OrderedLessonIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "lesson.reorder_failed");
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccess()
        {
            var lessonId = Guid.NewGuid();
            var command = new ReorderLessonsCommand(Guid.NewGuid(), InstructorId, [lessonId]);

            _lessonQueryMock
                .Setup(x => x.GetModuleAccessDataAsync(command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleAccessData(true, true));
            _lessonQueryMock
                .Setup(x => x.GetLessonIdsByModuleIdAsync(command.ModuleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([lessonId]);
            _lessonQueryMock
                .Setup(x => x.BulkUpdateDisplayOrderAsync(command.ModuleId, command.OrderedLessonIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
        }
    }
}
