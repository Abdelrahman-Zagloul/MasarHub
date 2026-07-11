using FluentAssertions;
using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Features.Courses.Queries.GetCourseById;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Courses.Queries.GetCourseById
{
    [Trait("UnitTests.Feature.Courses", "GetCourseById")]
    public sealed class GetCourseByIdQueryHandlerTests
    {
        private readonly Mock<ICourseQuery> _courseQueryMock;
        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly GetCourseByIdQueryHandler _sut;

        public GetCourseByIdQueryHandlerTests()
        {
            _courseQueryMock = new Mock<ICourseQuery>();
            _fileStorageServiceMock = new Mock<IFileStorageService>();
            _sut = new GetCourseByIdQueryHandler(_courseQueryMock.Object, _fileStorageServiceMock.Object);
        }

        [Fact]
        public async Task Handle_CourseNotFound_ReturnsNotFoundError()
        {
            var query = new GetCourseByIdQuery(Guid.NewGuid());

            _courseQueryMock
                .Setup(x => x.GetDetailsByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseDetailsResponse?)null);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_found");
        }

        [Fact]
        public async Task Handle_CourseFoundWithThumbnail_ResolvesThumbnailUrl()
        {
            var courseId = Guid.NewGuid();
            var query = new GetCourseByIdQuery(courseId);
            var course = new CourseDetailsResponse(
                courseId, "Title", "slug", "desc", 0, CourseLanguage.Arabic, CourseStatus.Published,
                CourseLevel.AllLevels, DateTimeOffset.UtcNow, Guid.NewGuid(), "Instructor",
                Guid.NewGuid(), "Category", "thumb-public-id", null
            );

            _courseQueryMock
                .Setup(x => x.GetDetailsByIdAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            _fileStorageServiceMock
                .Setup(x => x.GetUrl("thumb-public-id", FileType.Image))
                .Returns("https://example.com/thumb.jpg");

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.ThumbnailUrl.Should().Be("https://example.com/thumb.jpg");
        }

        [Fact]
        public async Task Handle_CourseFoundWithoutThumbnail_SetsNullUrl()
        {
            var courseId = Guid.NewGuid();
            var query = new GetCourseByIdQuery(courseId);
            var course = new CourseDetailsResponse(
                courseId, "Title", "slug", "desc", 0, CourseLanguage.Arabic, CourseStatus.Published,
                CourseLevel.AllLevels, DateTimeOffset.UtcNow, Guid.NewGuid(), "Instructor",
                Guid.NewGuid(), "Category", null, null
            );

            _courseQueryMock
                .Setup(x => x.GetDetailsByIdAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.ThumbnailUrl.Should().BeNull();
        }

        [Fact]
        public async Task Handle_CourseFound_ReturnsModulesWithTotals()
        {
            var courseId = Guid.NewGuid();
            var query = new GetCourseByIdQuery(courseId);
            var modules = new List<ModuleResponse>
            {
                new(Guid.NewGuid(), "Module 1", null, 1, 5),
                new(Guid.NewGuid(), "Module 2", null, 2, 3),
            };
            var course = new CourseDetailsResponse(
                courseId, "Title", "slug", "desc", 0, CourseLanguage.Arabic, CourseStatus.Published,
                CourseLevel.AllLevels, DateTimeOffset.UtcNow, Guid.NewGuid(), "Instructor",
                Guid.NewGuid(), "Category", null, null
            )
            { Modules = modules };

            _courseQueryMock
                .Setup(x => x.GetDetailsByIdAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.ModuleCount.Should().Be(2);
            result.Value.LessonCount.Should().Be(8);
            result.Value.Modules.Should().HaveCount(2);
            result.Value.Modules[0].LessonCount.Should().Be(5);
        }
    }
}
