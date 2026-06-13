using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Models.Storage;

namespace MasarHub.Application.ExternalServices
{
    public interface IMailService : ITransientService
    {
        Task SendEmailAsync(string to, string subject, string body, List<FileResource>? attachments = null, CancellationToken ct = default);
    }
}
