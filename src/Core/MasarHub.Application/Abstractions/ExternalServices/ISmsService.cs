using MasarHub.Application.Common.DI;
using MasarHub.Application.Common.Results;

namespace MasarHub.Application.Abstractions.ExternalServices
{
    public interface ISmsService : ITransientService
    {
        Task<Result> SendSmsAsync(string phoneNumber, string body);
    }
}
