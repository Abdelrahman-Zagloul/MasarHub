using MasarHub.Application.Common.DI;
using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Abstractions.Identity
{
    public interface ITwoFactorProvider : IScopedService
    {
        TwoFactorProvider Provider { get; }
        Task<Result> SendCodeAsync(Guid userId, CancellationToken ct = default);
    }
}
