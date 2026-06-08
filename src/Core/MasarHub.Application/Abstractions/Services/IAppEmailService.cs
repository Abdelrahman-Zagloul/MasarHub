using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Abstractions.Services
{
    public interface IAppEmailService : IScopedService
    {
        Task SendConfirmEmailAsync(string fullName, string email, string encodedToken);
        Task SendWelcomeEmailAsync(string fullName, string email, string role);
        Task SendPasswordChangedEmailAsync(string fullName, string email);
        Task SendPasswordResetEmailAsync(string fullName, string email, string encodedToken);
        Task SendTwoFactorEnabledEmailAsync(string fullName, string email, TwoFactorProvider provider);
        Task SendTwoFactorDisabledEmailAsync(string fullName, string email);
        Task SendTwoFactorCodeEmailAsync(string fullName, string email, string code);
        Task SendRecoveryCodeUsedEmailAsync(string fullName, string email);
        Task SendCourseApprovedEmailAsync(string fullName, string email, string courseTitle, string actionUrl);
        Task SendCourseRejectedEmailAsync(string fullName, string email, string courseTitle, string reason, string actionUrl);
    }
}
