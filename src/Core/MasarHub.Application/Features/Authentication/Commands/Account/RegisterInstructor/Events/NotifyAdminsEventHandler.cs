using MasarHub.Application.Abstractions.ExternalServices;
using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Common.Extensions;
using MasarHub.Application.Common.Models.Notifications;
using MasarHub.Domain.Modules.Notifications;
using MasarHub.Domain.Modules.Profiles;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Account.RegisterInstructor.Events
{
    public sealed class NotifyAdminsEventHandler : INotificationHandler<InstructorRegisteredEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IBackgroundJobService _backgroundJobService;
        public NotifyAdminsEventHandler(IUnitOfWork unitOfWork, IRepository<Notification> notificationRepository, INotificationRealtimeService notificationRealtimeService, IBackgroundJobService backgroundJobService)
        {
            _unitOfWork = unitOfWork;
            _notificationRepository = notificationRepository;
            _notificationRealtimeService = notificationRealtimeService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task Handle(InstructorRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var actionUrl = $"/admin/instructors/{notification.InstructorProfileId}";
            var notificationResult = Notification.CreateForRole(
                  targetRole: UserRole.Admin,
                  title: "New instructor registration",
                  message: $"{notification.FullName} registered as an instructor.",
                  type: NotificationType.InstructorRegistration,
                  priority: NotificationPriority.High,
                  actionUrl: actionUrl,
                  resourceId: notification.InstructorProfileId
            );

            if (notificationResult.IsFailure)
                return;

            await _notificationRealtimeService.SendToAdminsAsync(notificationResult.Value.ToRealtimeResponse(), cancellationToken);

            _backgroundJobService.Enqueue<ICreateNotificationJob>(x =>
                x.ExecuteAsync(CreateNotificationRequest.ForRole(notificationResult.Value)));
        }
    }
}
