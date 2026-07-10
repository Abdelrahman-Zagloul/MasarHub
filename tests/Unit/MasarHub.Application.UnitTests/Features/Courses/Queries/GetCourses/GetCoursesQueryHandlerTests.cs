using FluentAssertions;
using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Models.Storage;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Features.Courses.Queries.GetCourses;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Courses.Queries.GetCourses
{
    [Trait("UnitTests.Feature.Courses", "GetCourses")]
    public sealed class GetCoursesQueryHandlerTests
    {
        private readonly Mock<ICourseQuery> _courseQueryMock;
        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly GetCoursesQueryHandler _sut;

        public GetCoursesQueryHandlerTests()
        {
            _courseQueryMock = new Mock<ICourseQuery>();
            _fileStorageServiceMock = new Mock<IFileStorageService>();
            _sut = new GetCoursesQueryHandler(_courseQueryMock.Object, _fileStorageServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsPaginatedCoursesWithThumbnailUrls()
        {
            var query = new GetCoursesQuery(null, null, null, null, null, null, null, 1, 10);
            var courses = new List<CourseResponse>
            {
                new(Guid.NewGuid(), "Course 1", "slug-1", 10, "Arabic", "Published", "Beginner",
                    DateTimeOffset.UtcNow, Guid.NewGuid(), "Instr", Guid.NewGuid(), "Cat", "thumb-1"),
                new(Guid.NewGuid(), "Course 2", "slug-2", 20, "English", "Published", "Intermediate",
                    DateTimeOffset.UtcNow, Guid.NewGuid(), "Instr", Guid.NewGuid(), "Cat", null),
            };
            var pagedResult = new PagedResult<CourseResponse>(courses, 2);

            _courseQueryMock
                .Setup(x => x.GetAllAsync(query, CourseStatus.Published, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            _fileStorageServiceMock
                .Setup(x => x.GetUrl("thumb-1", FileType.Image))
                .Returns("https://example.com/thumb-1.jpg");

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(2);
            result.Value.Items[0].ThumbnailUrl.Should().Be("https://example.com/thumb-1.jpg");
            result.Value.Items[1].ThumbnailUrl.Should().BeNull();
            result.Value.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
        {
            var query = new GetCoursesQuery(null, null, null, null, null, null, null, 1, 10);
            var pagedResult = new PagedResult<CourseResponse>([], 0);

            _courseQueryMock
                .Setup(x => x.GetAllAsync(query, CourseStatus.Published, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().BeEmpty();
            result.Value.TotalCount.Should().Be(0);
        }
    }
}
