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
            var notificationResult = Notification.Create(
                    UserRole.Admin,
                    "New instructor registration",
                    $"{notification.FullName} registered as an instructor.",
                    NotificationType.InstructorRegistration,
                    NotificationPriority.High,
                    actionUrl,
                    notification.InstructorProfileId
            );

            if (notificationResult.IsFailure)
                return;

            await _notificationRepository.AddAsync(notificationResult.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var notifcation = notificationResult.Value;
            await _notificationRealtimeService.SendToAdminsAsync(new
            {
                notifcation.Id,
                notifcation.Title,
                notifcation.Message,
                notifcation.Type,
                notifcation.Priority,
                notifcation.ActionUrl,
                notifcation.CreatedAt
            }, cancellationToken);
        }
    }
}
