using FluentAssertions;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Features.Announcements.Queries.GetCourseAnnouncementByIdForInstructor;
using MasarHub.Application.Features.Announcements.Queries.GetInstructorCourseAnnouncements;
using MasarHub.Domain.Modules.Courses;
using Moq;

namespace MasarHub.Application.UnitTests.Features.Announcements.Queries.GetInstructorCourseAnnouncements
{
    [Trait("UnitTests.Feature.Announcements", "GetInstructorCourseAnnouncements")]
    public sealed class GetInstructorCourseAnnouncementsQueryHandlerTests
    {
        private readonly Mock<IAnnouncementQuery> _announcementQueryMock;
        private readonly GetInstructorCourseAnnouncementsQueryHandler _sut;

        public GetInstructorCourseAnnouncementsQueryHandlerTests()
        {
            _announcementQueryMock = new Mock<IAnnouncementQuery>();
            _sut = new GetInstructorCourseAnnouncementsQueryHandler(_announcementQueryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsPaginatedResult()
        {
            var query = new GetInstructorCourseAnnouncementsQuery(Guid.NewGuid(), Guid.NewGuid(), null, null, null, null, 1, 10);
            var items = new List<InstructorCourseAnnouncementResponse>
            {
                new(Guid.NewGuid(), query.CourseId, query.InstructorId, "Title", "Content", true,
                    DateTimeOffset.UtcNow, null, null, AnnouncementImportance.Normal,
                    false, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow),
                new(Guid.NewGuid(), query.CourseId, query.InstructorId, "Title 2", "Content 2", false,
                    null, null, null, AnnouncementImportance.High,
                    true, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow),
            };
            var pagedResult = new PagedResult<InstructorCourseAnnouncementResponse>(items, 2);

            _announcementQueryMock
                .Setup(x => x.GetInstructorListAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(2);
            result.Value.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task Handle_EmptyResult_ReturnsEmptyPaginatedResult()
        {
            var query = new GetInstructorCourseAnnouncementsQuery(Guid.NewGuid(), Guid.NewGuid(), null, null, null, null, 1, 10);
            var pagedResult = new PagedResult<InstructorCourseAnnouncementResponse>([], 0);

            _announcementQueryMock
                .Setup(x => x.GetInstructorListAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            var result = await _sut.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().BeEmpty();
            result.Value.TotalCount.Should().Be(0);
        }
    }
}
