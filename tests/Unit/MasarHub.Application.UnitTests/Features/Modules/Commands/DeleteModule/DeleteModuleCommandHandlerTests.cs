using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Modules.Commands.DeleteModule;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Modules.Commands.DeleteModule
{
    [Trait("UnitTests.Feature.Modules", "DeleteModule")]
    public sealed class DeleteModuleCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICourseModuleQuery> _courseModuleQueryMock;
        private readonly Mock<IRepository<CourseModule>> _courseModuleRepositoryMock;
        private readonly DeleteModuleCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public DeleteModuleCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _courseModuleQueryMock = new Mock<ICourseModuleQuery>();
            _courseModuleRepositoryMock = new Mock<IRepository<CourseModule>>();
            _sut = new DeleteModuleCommandHandler(_unitOfWorkMock.Object, _courseModuleQueryMock.Object, _courseModuleRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ModuleNotFound_ReturnsNotFoundError()
        {
            var command = new DeleteModuleCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId);

            _courseModuleQueryMock
                .Setup(x => x.GetDeleteDataAsync(command.CourseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleDeleteData(false, false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "module.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbiddenError()
        {
            var command = new DeleteModuleCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId);

            _courseModuleQueryMock
                .Setup(x => x.GetDeleteDataAsync(command.CourseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleDeleteData(true, false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_HasLessons_ReturnsBadRequestError()
        {
            var command = new DeleteModuleCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId);

            _courseModuleQueryMock
                .Setup(x => x.GetDeleteDataAsync(command.CourseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleDeleteData(true, true, true));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "module.cannot_delete.has_lessons");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ModuleNotFoundAfterCheck_ReturnsNotFoundError()
        {
            var command = new DeleteModuleCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId);

            _courseModuleQueryMock
                .Setup(x => x.GetDeleteDataAsync(command.CourseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleDeleteData(true, true, false));
            _courseModuleRepositoryMock
                .Setup(x => x.GetByIdAsync(command.ModuleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseModule?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "module.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidDelete_ReturnsSuccess()
        {
            var courseModule = CourseModule.Create(Guid.NewGuid(), "Introduction", 1, null).Value;
            var command = new DeleteModuleCommand(courseModule.CourseId, courseModule.Id, InstructorId);

            _courseModuleQueryMock
                .Setup(x => x.GetDeleteDataAsync(command.CourseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleDeleteData(true, true, false));
            _courseModuleRepositoryMock
                .Setup(x => x.GetByIdAsync(courseModule.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courseModule);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            courseModule.IsDeleted.Should().BeTrue();
            courseModule.DisplayOrder.Should().Be(0);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
