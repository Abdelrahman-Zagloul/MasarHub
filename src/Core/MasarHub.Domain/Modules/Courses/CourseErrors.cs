using MasarHub.Domain.Common.Errors;

namespace MasarHub.Domain.Modules.Courses
{
    public static class CourseErrors
    {
        public static readonly DomainError NotPendingApproval = new("course.not_pending_approval");
        public static readonly DomainError AlreadySubmitted = new("course.already_submitted");
        public static readonly DomainError AlreadyPublished = new("course.already_published");
        public static readonly DomainError AlreadyRejected = new("course.already_rejected");
        public static readonly DomainError InvalidRating = new("course_review.invalid_rating", "Rating");
        public static readonly DomainError AlreadyReviewed = new("course_review.already_reviewed");
    }

    public static class CourseEnrollmentErrors
    {
        public static readonly DomainError InvalidStatusTransition = new("course_enrollment.invalid_status_transition");
    }

    public static class CourseAnnouncementErrors
    {
        public static readonly DomainError AlreadyPublished = new("course_announcement.already_published");
        public static readonly DomainError CannotEditAfterPublish = new("course_announcement.cannot_edit_after_publish");
        public static readonly DomainError InvalidScheduleTime = new("course_announcement.invalid_schedule_time", "ScheduledAt");
        public static readonly DomainError InvalidExpirationTime = new("course_announcement.invalid_expiration_time", "ExpiresAt");
        public static readonly DomainError AlreadyScheduled = new("course_announcement.already_scheduled");
        public static readonly DomainError AlreadyPinned = new("course_announcement.already_pinned");
        public static readonly DomainError AlreadyUnpinned = new("course_announcement.already_unpinned");
    }
}
