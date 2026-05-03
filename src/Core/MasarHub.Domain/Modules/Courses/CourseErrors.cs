using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Modules.Courses
{
    public static class CourseErrors
    {
        public static readonly DomainError NotPendingApproval = new("Course.NotPendingApproval");
        public static readonly DomainError InvalidStatusTransition = new("Course.InvalidStatusTransition");
        public static readonly DomainError InvalidRating = new("CourseReview.InvalidRating", "Rating");
    }

    public static class CourseEnrollmentErrors
    {
        public static readonly DomainError InvalidStatusTransition = new("CourseEnrollment.InvalidStatusTransition");
    }

    public static class CourseAnnouncementErrors
    {
        public static readonly DomainError AlreadyPublished = new("CourseAnnouncement.AlreadyPublished");
        public static readonly DomainError CannotEditAfterPublish = new("CourseAnnouncement.CannotEditAfterPublish");
        public static readonly DomainError InvalidScheduleTime = new("CourseAnnouncement.InvalidScheduleTime", "ScheduledAt");
        public static readonly DomainError InvalidExpirationTime = new("CourseAnnouncement.InvalidExpirationTime", "ExpiresAt");
    }
}
