using MasarHub.Application.Abstractions.Jobs;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.Settings;
using Microsoft.Extensions.Options;

namespace MasarHub.Infrastructure.Jobs
{
    public sealed class EmailJob : IEmailJob
    {
        private readonly ICourseQuery _courseQuery;
        private readonly IAppEmailService _appEmailService;
        private readonly FrontendURLsSettings _frontendURLsSettings;

        public EmailJob(ICourseQuery courseQuery, IAppEmailService appEmailService, IOptions<FrontendURLsSettings> options)
        {
            _courseQuery = courseQuery;
            _appEmailService = appEmailService;
            _frontendURLsSettings = options.Value;
        }

        public async Task SendOrderCreatedEmailAsync(Guid userId, string orderNumber, decimal finalAmount, Guid orderId)
        {
            var userInfo = await _courseQuery.GetUserInfoAsync(userId);
            if (userInfo == null)
                return;

            await _appEmailService.SendOrderCreatedEmailAsync(
                userInfo.FullName,
                userInfo.Email,
                orderNumber,
                finalAmount.ToString("F2"),
                $"{_frontendURLsSettings.OrderCreatedPath}?orderId={orderId}");
        }

        public async Task SendOrderCancelledEmailAsync(Guid userId, string orderNumber, Guid orderId)
        {
            var userInfo = await _courseQuery.GetUserInfoAsync(userId);
            if (userInfo == null)
                return;

            await _appEmailService.SendOrderCancelledEmailAsync(
                userInfo.FullName,
                userInfo.Email,
                orderNumber,
                $"{_frontendURLsSettings.OrderCancelledPath}?orderId={orderId}");
        }

        public async Task SendPaymentSucceededEmailAsync(Guid userId, string orderNumber, decimal amount, Guid orderId)
        {
            var userInfo = await _courseQuery.GetUserInfoAsync(userId);
            if (userInfo == null)
                return;

            await _appEmailService.SendPaymentSucceededEmailAsync(
                userInfo.FullName,
                userInfo.Email,
                orderNumber,
                amount.ToString("F2"),
                $"/orders/{orderId}");
        }

        public async Task SendPaymentFailedEmailAsync(Guid userId, string orderNumber, decimal amount, Guid orderId)
        {
            var userInfo = await _courseQuery.GetUserInfoAsync(userId);
            if (userInfo == null)
                return;

            await _appEmailService.SendPaymentFailedEmailAsync(
                userInfo.FullName,
                userInfo.Email,
                orderNumber,
                amount.ToString("F2"),
                $"/orders/{orderId}");
        }

        public async Task SendCourseEnrollmentCreatedEmailAsync(Guid userId, string courseTitle, decimal paidAmount, Guid courseId)
        {
            var userInfo = await _courseQuery.GetUserInfoAsync(userId);
            if (userInfo == null)
                return;

            await _appEmailService.SendCourseEnrollmentCreatedEmailAsync(
                userInfo.FullName,
                userInfo.Email,
                courseTitle,
                paidAmount.ToString("F2"),
                $"/courses/{courseId}");
        }
    }
}
