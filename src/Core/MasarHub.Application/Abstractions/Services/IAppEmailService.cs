using MasarHub.Application.Common.DI;

namespace MasarHub.Application.Abstractions.Services
{
    public interface IAppEmailService : IScopedService
    {
        Task SendConfirmEmailAsync(string fullName, string email, string encodedToken);
        Task SendWelcomeEmailAsync(string fullName, string email, string role);
    }
}
