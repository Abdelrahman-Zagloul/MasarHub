using MasarHub.Application.Common.DI;
using MasarHub.Application.Common.Models;

namespace MasarHub.Application.ExternalServices
{
    public interface IMailService : ITransientService
    {
        Task SendEmailAsync(string to, string subject, string body, List<FileResource>? attachments = null, CancellationToken ct = default);
    }
}
