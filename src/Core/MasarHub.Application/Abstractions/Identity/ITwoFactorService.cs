using MasarHub.Application.Common.DI;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.DisableTwoFactor;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor;
using MasarHub.Application.Features.Authentication.Commands.TwoFactor.SetupAuthenticator;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Abstractions.Identity
{
    public interface ITwoFactorService : IScopedService
    {
        Task<Result<EnableTwoFactorResult>> EnableAsync(Guid userId, TwoFactorProvider provider);
        Task<Result<DisableTwoFactorResult>> DisableAsync(Guid userId);
        Task<Result> SendCodeAsync(Guid challengeId, CancellationToken ct = default);
        Task<Result<TokenUser>> VerifyCodeAsync(Guid challengeId, string code, CancellationToken ct = default);
        Task<Result<SetupAuthenticatorResult>> SetupAuthenticatorAsync(Guid userId);
        Task<Result<EnableTwoFactorResult>> VerifyAuthenticatorSetupAsync(Guid userId, string code);
    }
}
