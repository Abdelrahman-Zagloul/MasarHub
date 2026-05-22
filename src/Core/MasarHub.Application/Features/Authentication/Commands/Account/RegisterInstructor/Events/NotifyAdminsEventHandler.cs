using MasarHub.Application.Abstractions.Persistence;
using MasarHub.Application.Abstractions.Services;
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
        public NotifyAdminsEventHandler(IUnitOfWork unitOfWork, IRepository<Notification> notificationRepository, INotificationRealtimeService notificationRealtimeService)
        {
            _unitOfWork = unitOfWork;
            _notificationRepository = notificationRepository;
            _notificationRealtimeService = notificationRealtimeService;
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

            await _notificationRepository.AddAsync(notificationResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _notificationRealtimeService.SendToAdminsAsync(new
            {
                notificationResult.Value.Id,
                notificationResult.Value.Title,
                notificationResult.Value.Message,
                notificationResult.Value.Type,
                notificationResult.Value.Priority,
                notificationResult.Value.ActionUrl,
                notificationResult.Value.CreatedAt
            }, cancellationToken);
        }
    }
}
