using MasarHub.Application.Common.Models;
using MasarHub.Application.ExternalServices;
using MasarHub.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace MasarHub.Infrastructure.ExternalServices
{
    public sealed class MailService : IMailService
    {
        private readonly MailSettings _settings;
        private readonly ILogger<MailService> _logger;

        public MailService(IOptions<MailSettings> options, ILogger<MailService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, List<FileResource>? attachments = null, CancellationToken ct = default)
        {
            using var message = CreateMessage(to, subject, body, attachments);

            using var smtp = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_settings.Email, _settings.Password)
            };

            try
            {
                await smtp.SendMailAsync(message, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
                throw;
            }
        }

        private MailMessage CreateMessage(string to, string subject, string body, List<FileResource>? attachments)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_settings.Email, _settings.DisplayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(to));

            if (attachments is not null)
            {
                foreach (var file in attachments.Where(f => f.Content is not null))
                {
                    var stream = file.Content;
                    if (stream.CanSeek)
                        stream.Position = 0;
                    message.Attachments.Add(new Attachment(stream, file.FileName, file.ContentType));
                }
            }

            return message;
        }
    }
}