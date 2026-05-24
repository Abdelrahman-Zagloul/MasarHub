using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Models.Notifications;
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

        public async Task ExecuteAsync(CreateNotificationRequest request)
        {
            await _notificationRepository.AddAsync(request.ToNotification());
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
