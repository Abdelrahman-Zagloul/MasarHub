using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Modules.Commands.ReorderModules;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Modules.Commands.ReorderModules
{
    [Trait("UnitTests.Feature.Modules", "ReorderModules")]
    public sealed class ReorderModulesCommandHandlerTests
    {
        private readonly Mock<ICourseQuery> _courseQueryMock;
        private readonly Mock<ICourseModuleQuery> _courseModuleQueryMock;
        private readonly ReorderModulesCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public ReorderModulesCommandHandlerTests()
        {
            _courseQueryMock = new Mock<ICourseQuery>();
            _courseModuleQueryMock = new Mock<ICourseModuleQuery>();
            _sut = new ReorderModulesCommandHandler(_courseQueryMock.Object, _courseModuleQueryMock.Object);
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsForbiddenError()
        {
            var command = new ReorderModulesCommand(Guid.NewGuid(), InstructorId, [Guid.NewGuid()]);

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(command.CourseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_found");
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbiddenError()
        {
            var command = new ReorderModulesCommand(Guid.NewGuid(), InstructorId, [Guid.NewGuid()]);

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(command.CourseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
        }

        [Fact]
        public async Task Handle_ModuleCountMismatch_ReturnsBadRequestError()
        {
            var courseId = Guid.NewGuid();
            var moduleId = Guid.NewGuid();
            var command = new ReorderModulesCommand(courseId, InstructorId, [moduleId]);

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(courseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, true));
            _courseModuleQueryMock
                .Setup(x => x.GetModuleIdsByCourseIdAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([moduleId, Guid.NewGuid()]);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "module.reorder_items_mismatch");
        }

        [Fact]
        public async Task Handle_ModuleNotFoundInCourse_ReturnsBadRequestError()
        {
            var courseId = Guid.NewGuid();
            var existingId = Guid.NewGuid();
            var wrongId = Guid.NewGuid();
            var command = new ReorderModulesCommand(courseId, InstructorId, [wrongId]);

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(courseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, true));
            _courseModuleQueryMock
                .Setup(x => x.GetModuleIdsByCourseIdAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([existingId]);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "module.reorder_module_not_found");
        }

        [Fact]
        public async Task Handle_ValidReorder_ReturnsSuccess()
        {
            var courseId = Guid.NewGuid();
            var moduleId1 = Guid.NewGuid();
            var moduleId2 = Guid.NewGuid();
            var orderedIds = new List<Guid> { moduleId1, moduleId2 }.AsReadOnly();
            var command = new ReorderModulesCommand(courseId, InstructorId, orderedIds);

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(courseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, true));
            _courseModuleQueryMock
                .Setup(x => x.GetModuleIdsByCourseIdAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([moduleId1, moduleId2]);
            _courseModuleQueryMock
                .Setup(x => x.BulkUpdateDisplayOrderAsync(courseId, orderedIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _courseModuleQueryMock.Verify(x => x.BulkUpdateDisplayOrderAsync(courseId, orderedIds, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_BulkUpdateFails_ReturnsFailureError()
        {
            var courseId = Guid.NewGuid();
            var moduleId = Guid.NewGuid();
            var orderedIds = new List<Guid> { moduleId }.AsReadOnly();
            var command = new ReorderModulesCommand(courseId, InstructorId, orderedIds);

            _courseQueryMock
                .Setup(x => x.GetCourseAccessData(courseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CourseAccessData(true, true));
            _courseModuleQueryMock
                .Setup(x => x.GetModuleIdsByCourseIdAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync([moduleId]);
            _courseModuleQueryMock
                .Setup(x => x.BulkUpdateDisplayOrderAsync(courseId, orderedIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "module.reorder_failed");
        }
    }
}
