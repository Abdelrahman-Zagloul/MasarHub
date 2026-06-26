using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Jobs
{
    public interface IEmailJob : IScopedService
    {
        Task SendOrderCreatedEmailAsync(Guid userId, string orderNumber, decimal finalAmount, Guid orderId);
        Task SendOrderCancelledEmailAsync(Guid userId, string orderNumber, Guid orderId);
        Task SendPaymentSucceededEmailAsync(Guid userId, string orderNumber, decimal amount, Guid orderId);
        Task SendPaymentFailedEmailAsync(Guid userId, string orderNumber, decimal amount, Guid orderId);
        Task SendCourseEnrollmentCreatedEmailAsync(Guid userId, string courseTitle, decimal paidAmount, Guid courseId);
    }
}
