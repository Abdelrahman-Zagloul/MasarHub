using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyTwoFactorCode
{
    public sealed record VerifyTwoFactorCodeCommand(Guid ChallengeId, string Code)
        : IRequest<Result<AccessWithRefreshTokenResult>>;
}
