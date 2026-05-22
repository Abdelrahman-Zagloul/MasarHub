using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Persistence;
using MasarHub.Domain.Modules.Notifications;

namespace MasarHub.Infrastructure.Jobs
{
    public sealed class CreateNotificationJob : ICreateNotificationJob
    {
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateNotificationJob(IRepository<Notification> notificationRepository, IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(Notification notification)
        {
            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
