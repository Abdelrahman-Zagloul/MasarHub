using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Common.DomainEvents;
using MasarHub.Domain.Modules.Courses.Events;
using MediatR;

namespace MasarHub.Application.Features.Reviews.Commands.CreateCourseReview.Events
{
    public sealed class CourseReviewCreatedNotificationEventHandler
        : INotificationHandler<DomainEventNotification<CourseReviewCreatedDomainEvent>>
    {
        private readonly IBackgroundJobService _backgroundJobService;

        public CourseReviewCreatedNotificationEventHandler(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }

        public Task Handle(DomainEventNotification<CourseReviewCreatedDomainEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _backgroundJobService.Enqueue<IReviewJob>(x =>
                x.NotifyInstructorOfNewReviewAsync(domainEvent.CourseId, domainEvent.ReviewId, domainEvent.Rating));

            return Task.CompletedTask;
        }
    }
}
