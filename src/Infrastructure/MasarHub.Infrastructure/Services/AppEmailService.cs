using MasarHub.Application.Abstractions.Services;
using MasarHub.Application.ExternalServices;
using MasarHub.Application.Settings;
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
    }
}
