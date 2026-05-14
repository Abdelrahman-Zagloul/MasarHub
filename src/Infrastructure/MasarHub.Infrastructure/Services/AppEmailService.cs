using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.ExternalServices;
using MasarHub.Application.Settings;
using MasarHub.Domain.Modules.Profiles;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace MasarHub.Infrastructure.Services
{
    public sealed class AppEmailService : IAppEmailService
    {
        private readonly IMailService _mailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FrontendURLsSettings _settings;

        public AppEmailService(IMailService mailService, IWebHostEnvironment webHostEnvironment, IOptions<FrontendURLsSettings> options)
        {
            _mailService = mailService;
            _webHostEnvironment = webHostEnvironment;
            _settings = options.Value;
        }

        public async Task SendConfirmEmailAsync(string fullName, string email, string encodedToken)
        {
            var confirmationLink = $"{_settings.ConfirmEmailPath}?email={email}&token={encodedToken}";
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", "ConfirmEmail.html");
            var templateContent = await File.ReadAllTextAsync(path);

            var emailBody = templateContent
                .Replace("{FullName}", fullName)
                .Replace("{confirmationLink}", confirmationLink)
                .Replace("{token}", encodedToken);

            await _mailService.SendEmailAsync(email, "Confirm Email", emailBody, null);
        }

        public async Task SendPasswordChangedEmailAsync(string fullName, string email)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", "PasswordChanged.html");
            var templateContent = await File.ReadAllTextAsync(path);
            var emailBody = templateContent.Replace("{FullName}", fullName);

            await _mailService.SendEmailAsync(email, "Password Changed Successfully", emailBody, null);
        }

        public async Task SendPasswordResetEmailAsync(string fullName, string email, string encodedToken)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", "ResetPassword.html");

            var resetLink = $"{_settings.ResetPasswordPath}?email={email}&token={encodedToken}";

            var templateContent = await File.ReadAllTextAsync(path);
            var emailBody = templateContent
                .Replace("{FullName}", fullName)
                .Replace("{ResetLink}", resetLink)
                .Replace("{Token}", encodedToken);

            await _mailService.SendEmailAsync(email, "Reset Password", emailBody, null);
        }

        public async Task SendWelcomeEmailAsync(string fullName, string email, string role)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", "WelcomeEmail.html");
            var templateContent = await File.ReadAllTextAsync(path);

            var roleMessage = role == "Instructor"
                ? "Your instructor account is now created. Our admin team will review your profile, and you will be notified once it is approved."
                : "Your account is ready. You can start exploring courses after confirming your email.";

            var emailBody = templateContent
                .Replace("{FullName}", fullName)
                .Replace("{RoleMessage}", roleMessage)
                .Replace("{FrontendUrl}", _settings.BaseURL);

            await _mailService.SendEmailAsync(email, "Welcome to MasarHub", emailBody, null);
        }

        public async Task SendTwoFactorEnabledEmailAsync(string fullName, string email, TwoFactorProvider provider)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", "TwoFactorEnabled.html");
            var templateContent = await File.ReadAllTextAsync(path);

            var emailBody = templateContent
                .Replace("{FullName}", fullName)
                .Replace("{Provider}", provider.ToString());

            await _mailService.SendEmailAsync(email, "Two Factor Authentication Enabled", emailBody, null);
        }

        public async Task SendTwoFactorDisabledEmailAsync(string fullName, string email)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", "TwoFactorDisabled.html");

            var templateContent = await File.ReadAllTextAsync(path);

            var emailBody = templateContent
                .Replace("{FullName}", fullName);

            await _mailService.SendEmailAsync(email, "Two-Factor Authentication Disabled", emailBody, null);
        }
    }
}
