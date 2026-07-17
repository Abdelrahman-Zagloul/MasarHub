using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Modules.Courses;
using MasarHub.Domain.Modules.Courses.Events;

namespace MasarHub.Domain.UnitTests.Modules.Courses
{
    [Trait("UnitTests.Domain.Courses", "CourseAnnouncement")]
    public sealed class CourseAnnouncementTests
    {
        private static readonly Guid ValidCourseId = Guid.NewGuid();
        private static readonly Guid ValidInstructorId = Guid.NewGuid();
        private const string ValidTitle = "Important Announcement";
        private const string ValidContent = "This is an important announcement for all students.";

        #region Create

        [Fact]
        public void Create_ValidInput_ReturnsSuccess()
        {
            var result = CourseAnnouncement.Create(ValidCourseId, ValidInstructorId, ValidTitle, ValidContent, AnnouncementImportance.High);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(ValidCourseId, result.Value.CourseId);
            Assert.Equal(ValidInstructorId, result.Value.InstructorId);
            Assert.Equal(ValidTitle, result.Value.Title);
            Assert.Equal(ValidContent, result.Value.Content);
            Assert.Equal(AnnouncementImportance.High, result.Value.Importance);
            Assert.False(result.Value.IsPublished);
            Assert.False(result.Value.IsPinned);
            Assert.Empty(result.Value.DomainEvents);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidTitle_ReturnsError(string? title)
        {
            var result = CourseAnnouncement.Create(ValidCourseId, ValidInstructorId, title!, ValidContent, AnnouncementImportance.Normal);

            Assert.True(result.IsFailure);
            Assert.NotNull(result.Error);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_InvalidContent_ReturnsError(string? content)
        {
            var result = CourseAnnouncement.Create(ValidCourseId, ValidInstructorId, ValidTitle, content!, AnnouncementImportance.Normal);

            Assert.True(result.IsFailure);
            Assert.NotNull(result.Error);
        }

        [Fact]
        public void Create_EmptyCourseId_ReturnsError()
        {
            var result = CourseAnnouncement.Create(Guid.Empty, ValidInstructorId, ValidTitle, ValidContent, AnnouncementImportance.Normal);

            Assert.True(result.IsFailure);
            Assert.NotNull(result.Error);
        }

        [Fact]
        public void Create_EmptyInstructorId_ReturnsError()
        {
            var result = CourseAnnouncement.Create(ValidCourseId, Guid.Empty, ValidTitle, ValidContent, AnnouncementImportance.Normal);

            Assert.True(result.IsFailure);
            Assert.NotNull(result.Error);
        }

        [Fact]
        public void Create_InvalidImportance_ReturnsError()
        {
            var result = CourseAnnouncement.Create(ValidCourseId, ValidInstructorId, ValidTitle, ValidContent, (AnnouncementImportance)99);

            Assert.True(result.IsFailure);
            Assert.NotNull(result.Error);
        }

        #endregion

        #region UpdateTitle

        [Fact]
        public void UpdateTitle_ValidInput_UpdatesTitle()
        {
            var announcement = CreateValidAnnouncement();
            var newTitle = "Updated Announcement Title";

            var result = announcement.UpdateTitle(newTitle);

            Assert.True(result.IsSuccess);
            Assert.Equal(newTitle, announcement.Title);
        }

        [Fact]
        public void UpdateTitle_Published_ReturnsCannotEditError()
        {
            var announcement = CreateValidAnnouncement();
            announcement.Publish();

            var result = announcement.UpdateTitle("New Title");

            Assert.True(result.IsFailure);
            Assert.Equal(CourseAnnouncementErrors.CannotEditAfterPublish.Code, result.Error.Code);
        }

        #endregion

        #region UpdateContent

        [Fact]
        public void UpdateContent_ValidInput_UpdatesContent()
        {
            var announcement = CreateValidAnnouncement();
            var newContent = "Updated announcement content here.";

            var result = announcement.UpdateContent(newContent);

            Assert.True(result.IsSuccess);
            Assert.Equal(newContent, announcement.Content);
        }

        [Fact]
        public void UpdateContent_Published_ReturnsCannotEditError()
        {
            var announcement = CreateValidAnnouncement();
            announcement.Publish();

            var result = announcement.UpdateContent("New Content");

            Assert.True(result.IsFailure);
            Assert.Equal(CourseAnnouncementErrors.CannotEditAfterPublish.Code, result.Error.Code);
        }

        #endregion

        #region SetImportance

        [Fact]
        public void SetImportance_ValidInput_UpdatesImportance()
        {
            var announcement = CreateValidAnnouncement();

            var result = announcement.SetImportance(AnnouncementImportance.High);

            Assert.True(result.IsSuccess);
            Assert.Equal(AnnouncementImportance.High, announcement.Importance);
        }

        [Fact]
        public void SetImportance_InvalidEnum_ReturnsError()
        {
            var announcement = CreateValidAnnouncement();

            var result = announcement.SetImportance((AnnouncementImportance)99);

            Assert.True(result.IsFailure);
            Assert.NotNull(result.Error);
        }

        #endregion

        #region Schedule

        [Fact]
        public void Schedule_ValidFutureTime_SchedulesAnnouncement()
        {
            var announcement = CreateValidAnnouncement();
            var scheduledAt = DateTimeOffset.UtcNow.AddDays(7);

            var result = announcement.Schedule(scheduledAt);

            Assert.True(result.IsSuccess);
            Assert.Equal(scheduledAt, announcement.ScheduledAt);
            Assert.Single(announcement.DomainEvents);
            Assert.IsType<CourseAnnouncementScheduledDomainEvent>(announcement.DomainEvents.First());
        }

        [Fact]
        public void Schedule_AlreadyScheduled_ReturnsError()
        {
            var announcement = CreateValidAnnouncement();
            announcement.Schedule(DateTimeOffset.UtcNow.AddDays(7));

            var result = announcement.Schedule(DateTimeOffset.UtcNow.AddDays(14));

            Assert.True(result.IsFailure);
            Assert.Equal(CourseAnnouncementErrors.AlreadyScheduled.Code, result.Error.Code);
        }

        [Fact]
        public void Schedule_PastTime_ReturnsError()
        {
            var announcement = CreateValidAnnouncement();

            var result = announcement.Schedule(DateTimeOffset.UtcNow.AddDays(-1));

            Assert.True(result.IsFailure);
            Assert.Equal(CourseAnnouncementErrors.InvalidScheduleTime.Code, result.Error.Code);
        }

        [Fact]
        public void Schedule_Now_ReturnsError()
        {
            var announcement = CreateValidAnnouncement();

            var result = announcement.Schedule(DateTimeOffset.UtcNow);

            Assert.True(result.IsFailure);
            Assert.Equal(CourseAnnouncementErrors.InvalidScheduleTime.Code, result.Error.Code);
        }

        [Fact]
        public void Schedule_Published_ReturnsAlreadyPublishedError()
        {
            var announcement = CreateValidAnnouncement();
            announcement.Publish();

            var result = announcement.Schedule(DateTimeOffset.UtcNow.AddDays(7));

            Assert.True(result.IsFailure);
            Assert.Equal(CourseAnnouncementErrors.AlreadyPublished.Code, result.Error.Code);
        }

        #endregion

        #region Publish

        [Fact]
        public void Publish_ValidDraft_PublishesAnnouncement()
        {
            var announcement = CreateValidAnnouncement();

            var result = announcement.Publish();

            Assert.True(result.IsSuccess);
            Assert.True(announcement.IsPublished);
            Assert.NotNull(announcement.PublishedAt);
            Assert.Single(announcement.DomainEvents);
            Assert.IsType<CourseAnnouncementPublishedDomainEvent>(announcement.DomainEvents.First());
        }

        [Fact]
        public void Publish_AlreadyPublished_ReturnsError()
        {
            var announcement = CreateValidAnnouncement();
            announcement.Publish();

            var result = announcement.Publish();

            Assert.True(result.IsFailure);
            Assert.Equal(CourseAnnouncementErrors.AlreadyPublished.Code, result.Error.Code);
        }

        #endregion

        #region Pin

        [Fact]
        public void Pin_NotPinned_PinsAnnouncement()
        {
            var announcement = CreateValidAnnouncement();

            var result = announcement.Pin();

            Assert.True(result.IsSuccess);
            Assert.True(announcement.IsPinned);
        }

        [Fact]
        public void Pin_AlreadyPinned_ReturnsError()
        {
            var announcement = CreateValidAnnouncement();
            announcement.Pin();

            var result = announcement.Pin();

            Assert.True(result.IsFailure);
            Assert.Equal(CourseAnnouncementErrors.AlreadyPinned.Code, result.Error.Code);
        }

        #endregion

        #region Unpin

        [Fact]
        public void Unpin_Pinned_UnpinsAnnouncement()
        {
            var announcement = CreateValidAnnouncement();
            announcement.Pin();

            var result = announcement.Unpin();

            Assert.True(result.IsSuccess);
            Assert.False(announcement.IsPinned);
        }

        [Fact]
        public void Unpin_AlreadyUnpinned_ReturnsError()
        {
            var announcement = CreateValidAnnouncement();

            var result = announcement.Unpin();

            Assert.True(result.IsFailure);
            Assert.Equal(CourseAnnouncementErrors.AlreadyUnpinned.Code, result.Error.Code);
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_NotDeleted_MarksAsDeleted()
        {
            var announcement = CreateValidAnnouncement();

            var result = announcement.Delete();

            Assert.True(result.IsSuccess);
            Assert.True(announcement.IsDeleted);
            Assert.NotNull(announcement.DeletedAt);
        }

        [Fact]
        public void Delete_AlreadyDeleted_ReturnsError()
        {
            var announcement = CreateValidAnnouncement();
            announcement.Delete();

            var result = announcement.Delete();

            Assert.True(result.IsFailure);
            Assert.Equal(DomainError.AlreadyDeleted().Code, result.Error.Code);
        }

        #endregion

        #region SetExpiration

        [Fact]
        public void SetExpiration_ValidFutureTime_SetsExpiration()
        {
            var announcement = CreateValidAnnouncement();
            var expiresAt = DateTimeOffset.UtcNow.AddDays(30);

            var result = announcement.SetExpiration(expiresAt);

            Assert.True(result.IsSuccess);
            Assert.Equal(expiresAt, announcement.ExpiresAt);
        }

        [Fact]
        public void SetExpiration_PastTime_ReturnsError()
        {
            var announcement = CreateValidAnnouncement();

            var result = announcement.SetExpiration(DateTimeOffset.UtcNow.AddDays(-1));

            Assert.True(result.IsFailure);
            Assert.Equal(CourseAnnouncementErrors.InvalidExpirationTime.Code, result.Error.Code);
        }

        #endregion

        private static CourseAnnouncement CreateValidAnnouncement()
        {
            return CourseAnnouncement.Create(ValidCourseId, ValidInstructorId, ValidTitle, ValidContent, AnnouncementImportance.Normal).Value;
        }
    }
}
