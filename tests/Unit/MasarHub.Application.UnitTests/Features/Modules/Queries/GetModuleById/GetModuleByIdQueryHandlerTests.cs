using FluentAssertions;
using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Features.Modules.Queries.GetModuleById;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Modules.Queries.GetModuleById
{
    [Trait("UnitTests.Feature.Modules", "GetModuleById")]
    public sealed class GetModuleByIdQueryHandlerTests
    {
        private readonly Mock<ICourseModuleQuery> _courseModuleQueryMock;
        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly GetModuleByIdQueryHandler _sut;

        public GetModuleByIdQueryHandlerTests()
        {
            _courseModuleQueryMock = new Mock<ICourseModuleQuery>();
            _fileStorageServiceMock = new Mock<IFileStorageService>();
            _sut = new GetModuleByIdQueryHandler(_courseModuleQueryMock.Object, _fileStorageServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ModuleNotFound_ReturnsNotFoundError()
        {
            var query = new GetModuleByIdQuery(Guid.NewGuid(), Guid.NewGuid());

            _courseModuleQueryMock
                .Setup(x => x.GetModuleByIdAsync(query.CourseId, query.ModuleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ModuleDetailsResponse?)null);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "module.not_found");
        }

        [Fact]
        public async Task Handle_ModuleWithPreviewableLessons_ResolvesVideoUrls()
        {
            var courseId = Guid.NewGuid();
            var moduleId = Guid.NewGuid();
            var query = new GetModuleByIdQuery(courseId, moduleId);
            var module = new ModuleDetailsResponse(moduleId, "Module 1", null, 1)
            {
                Lessons =
                [
                    new LessonResponse(Guid.NewGuid(), "Lesson 1", null, 1, true, "video", "video-public-id"),
                    new LessonResponse(Guid.NewGuid(), "Lesson 2", null, 2, false, "article", null),
                ]
            };

            _courseModuleQueryMock
                .Setup(x => x.GetModuleByIdAsync(courseId, moduleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(module);

            _fileStorageServiceMock
                .Setup(x => x.GetUrl("video-public-id", FileType.Video))
                .Returns("https://example.com/video.mp4");

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Lessons.Should().HaveCount(2);
            result.Value.Lessons[0].VideoUrl.Should().Be("https://example.com/video.mp4");
            result.Value.Lessons[1].VideoUrl.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ModuleWithoutPreviewableLessons_SkipsUrlResolution()
        {
            var courseId = Guid.NewGuid();
            var moduleId = Guid.NewGuid();
            var query = new GetModuleByIdQuery(courseId, moduleId);
            var module = new ModuleDetailsResponse(moduleId, "Module 1", null, 1)
            {
                Lessons =
                [
                    new LessonResponse(Guid.NewGuid(), "Lesson 1", null, 1, false, "article", null),
                ]
            };

            _courseModuleQueryMock
                .Setup(x => x.GetModuleByIdAsync(courseId, moduleId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(module);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Lessons[0].VideoUrl.Should().BeNull();
            _fileStorageServiceMock.Verify(x => x.GetUrl(It.IsAny<string>(), It.IsAny<FileType>()), Times.Never);
        }
    }
}
