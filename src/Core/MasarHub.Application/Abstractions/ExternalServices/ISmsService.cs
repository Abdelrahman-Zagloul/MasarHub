using MasarHub.Application.Common.DependencyInjection;
using MasarHub.Application.Common.Results;

namespace MasarHub.Application.Abstractions.ExternalServices
{
    public interface ISmsService : ITransientService
    {
        Task<Result> SendSmsAsync(string phoneNumber, string body);
    }
}
