using MasarHub.Application.Common.DI;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Abstractions.Identity
{
    public interface ITwoFactorService : IScopedService
    {
        Task<Result<EnableTwoFactorResult>> EnableAsync(Guid userId, TwoFactorProvider provider);
    }
}
