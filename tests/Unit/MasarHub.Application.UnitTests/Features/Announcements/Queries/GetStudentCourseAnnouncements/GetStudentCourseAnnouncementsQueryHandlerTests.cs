using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForStudent;
using MasarHub.Application.Features.Announcements.Queries.GetStudentCourseAnnouncements;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Announcements.Queries.GetStudentCourseAnnouncements
{
    [Trait("UnitTests.Feature.Announcements", "GetStudentCourseAnnouncements")]
    public sealed class GetStudentCourseAnnouncementsQueryHandlerTests
    {
        private readonly Mock<IAnnouncementQuery> _announcementQueryMock;
        private readonly GetStudentCourseAnnouncementsQueryHandler _sut;

        public GetStudentCourseAnnouncementsQueryHandlerTests()
        {
            _announcementQueryMock = new Mock<IAnnouncementQuery>();
            _sut = new GetStudentCourseAnnouncementsQueryHandler(_announcementQueryMock.Object);
        }

        [Fact]
        public async Task Handle_Enrolled_ReturnsPaginatedResult()
        {
            var query = new GetStudentCourseAnnouncementsQuery(Guid.NewGuid(), Guid.NewGuid(), null, null, null, 1, 10);
            var items = new List<StudentCourseAnnouncementResponse>
            {
                new(Guid.NewGuid(), query.CourseId, "Title", "Content", DateTimeOffset.UtcNow,
                    AnnouncementImportance.Normal, false, DateTimeOffset.UtcNow),
            };
            var pagedResult = new PagedResult<StudentCourseAnnouncementResponse>(items, 1);

            _announcementQueryMock
                .Setup(x => x.GetStudentListAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StudentAnnouncementPaginatedResult(true, pagedResult));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(1);
            result.Value.TotalCount.Should().Be(1);
        }

        [Fact]
        public async Task Handle_NotEnrolled_ReturnsForbiddenError()
        {
            var query = new GetStudentCourseAnnouncementsQuery(Guid.NewGuid(), Guid.NewGuid(), null, null, null, 1, 10);

            _announcementQueryMock
                .Setup(x => x.GetStudentListAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StudentAnnouncementPaginatedResult(false, null));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Code == "course.not_enrolled");
        }

        [Fact]
        public async Task Handle_EnrolledEmpty_ReturnsEmptyPaginatedResult()
        {
            var query = new GetStudentCourseAnnouncementsQuery(Guid.NewGuid(), Guid.NewGuid(), null, null, null, 1, 10);
            var pagedResult = new PagedResult<StudentCourseAnnouncementResponse>([], 0);

            _announcementQueryMock
                .Setup(x => x.GetStudentListAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StudentAnnouncementPaginatedResult(true, pagedResult));

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().BeEmpty();
            result.Value.TotalCount.Should().Be(0);
        }
    }
}
