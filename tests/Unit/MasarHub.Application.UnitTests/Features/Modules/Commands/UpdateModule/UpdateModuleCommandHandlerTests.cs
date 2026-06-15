using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Features.Modules.Commands.UpdateModule;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Modules.Commands.UpdateModule
{
    [Trait("UnitTests.Feature.Modules", "UpdateModule")]
    public sealed class UpdateModuleCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<CourseModule>> _courseModuleRepositoryMock;
        private readonly Mock<ICourseModuleQuery> _courseModuleQueryMock;
        private readonly UpdateModuleCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public UpdateModuleCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _courseModuleRepositoryMock = new Mock<IRepository<CourseModule>>();
            _courseModuleQueryMock = new Mock<ICourseModuleQuery>();
            _sut = new UpdateModuleCommandHandler(_unitOfWorkMock.Object, _courseModuleRepositoryMock.Object, _courseModuleQueryMock.Object);
        }

        [Fact]
        public async Task Handle_ModuleNotFound_ReturnsNotFoundError()
        {
            var command = new UpdateModuleCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, "New Title", null);

            _courseModuleQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.CourseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleUpdateData(false, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "module.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbiddenError()
        {
            var command = new UpdateModuleCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, "New Title", null);

            _courseModuleQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.CourseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleUpdateData(true, false));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ModuleNotFoundAfterCheck_ReturnsNotFoundError()
        {
            var command = new UpdateModuleCommand(Guid.NewGuid(), Guid.NewGuid(), InstructorId, "New Title", null);

            _courseModuleQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.CourseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleUpdateData(true, true));
            _courseModuleRepositoryMock
                .Setup(x => x.GetByIdAsync(command.ModuleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseModule?)null);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "module.not_found");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UpdateTitle_UpdatesTitle()
        {
            var courseModule = CreateModule();
            var command = new UpdateModuleCommand(courseModule.CourseId, courseModule.Id, InstructorId, "New Title", null);

            _courseModuleQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.CourseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleUpdateData(true, true));
            _courseModuleRepositoryMock
                .Setup(x => x.GetByIdAsync(courseModule.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courseModule);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            courseModule.Title.Should().Be("New Title");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateDescription_UpdatesDescription()
        {
            var courseModule = CreateModule();
            var command = new UpdateModuleCommand(courseModule.CourseId, courseModule.Id, InstructorId, null, "New description");

            _courseModuleQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.CourseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleUpdateData(true, true));
            _courseModuleRepositoryMock
                .Setup(x => x.GetByIdAsync(courseModule.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courseModule);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            courseModule.Description.Should().Be("New description");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NoChanges_StillSaves()
        {
            var courseModule = CreateModule();
            var command = new UpdateModuleCommand(courseModule.CourseId, courseModule.Id, InstructorId, null, null);

            _courseModuleQueryMock
                .Setup(x => x.GetUpdateDataAsync(command.CourseId, command.ModuleId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleUpdateData(true, true));
            _courseModuleRepositoryMock
                .Setup(x => x.GetByIdAsync(courseModule.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courseModule);

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private static CourseModule CreateModule()
        {
            return CourseModule.Create(Guid.NewGuid(), "Original Title", 1, "Original description").Value;
        }
    }
}
