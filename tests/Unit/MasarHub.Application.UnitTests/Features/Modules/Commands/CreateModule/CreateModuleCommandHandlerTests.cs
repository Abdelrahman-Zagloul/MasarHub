using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Modules.Commands.CreateModule;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Modules.Commands.CreateModule
{
    [Trait("UnitTests.Feature.Modules", "CreateModule")]
    public sealed class CreateModuleCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICourseModuleQuery> _moduleQueryMock;
        private readonly Mock<IRepository<CourseModule>> _moduleRepositoryMock;
        private readonly CreateModuleCommandHandler _sut;
        private static readonly Guid InstructorId = Guid.NewGuid();

        public CreateModuleCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _moduleQueryMock = new Mock<ICourseModuleQuery>();
            _moduleRepositoryMock = new Mock<IRepository<CourseModule>>();
            _sut = new CreateModuleCommandHandler(_unitOfWorkMock.Object, _moduleQueryMock.Object, _moduleRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsNotFoundError()
        {
            var command = new CreateModuleCommand(Guid.NewGuid(), InstructorId, "Introduction", "Module description");

            _moduleQueryMock
                .Setup(x => x.GetCreationDataAsync(command.CourseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleCreationData(false, false, 1));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_found");
            _moduleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CourseModule>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NotOwner_ReturnsForbiddenError()
        {
            var command = new CreateModuleCommand(Guid.NewGuid(), InstructorId, "Introduction", "Module description");

            _moduleQueryMock
                .Setup(x => x.GetCreationDataAsync(command.CourseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleCreationData(true, false, 1));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.access_denied");
            _moduleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CourseModule>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DomainFailure_ReturnsFailure()
        {
            var command = new CreateModuleCommand(Guid.Empty, InstructorId, "Introduction", null);

            _moduleQueryMock
                .Setup(x => x.GetCreationDataAsync(Guid.Empty, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleCreationData(true, true, 1));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            _moduleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CourseModule>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidModule_ReturnsSuccessResponse()
        {
            var courseId = Guid.NewGuid();
            var command = new CreateModuleCommand(courseId, InstructorId, "Introduction", "Module description");

            _moduleQueryMock
                .Setup(x => x.GetCreationDataAsync(courseId, InstructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ModuleCreationData(true, true, 1));

            var result = await _sut.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Title.Should().Be("Introduction");
            result.Value.DisplayOrder.Should().Be(1);
            result.Value.CourseId.Should().Be(courseId);
            _moduleRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CourseModule>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
