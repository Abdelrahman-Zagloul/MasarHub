using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Extensions;
using MasarHub.Domain.Modules.Notifications;

namespace MasarHub.Infrastructure.Jobs
{
    public sealed class ReviewJob : IReviewJob
    {
        private readonly ICourseQuery _courseQuery;
        private readonly ICreateNotificationJob _createNotificationJob;

        public ReviewJob(ICourseQuery courseQuery, ICreateNotificationJob createNotificationJob)
        {
            _courseQuery = courseQuery;
            _createNotificationJob = createNotificationJob;
        }

        public async Task NotifyInstructorOfNewReviewAsync(Guid courseId, Guid reviewId, double rating)
        {
            var course = await _courseQuery.GetDetailsByIdAsync(courseId);
            if (course is null)
                return;

            var notificationResult = Notification.CreateForUser(
                userId: course.InstructorId,
                title: "New Course Review",
                message: $"A student left a {rating}-star review on your course.",
                type: NotificationType.CourseReviewCreated,
                priority: NotificationPriority.Normal,
                actionUrl: $"/courses/{courseId}/reviews/{reviewId}",
                resourceId: reviewId);

            if (notificationResult.IsFailure)
                return;

            await _createNotificationJob.ExecuteAsync(notificationResult.Value.ToCreateRequest());
        }
    }
}
