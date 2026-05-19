using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.SendCode
{
    public sealed record SendTwoFactorCodeCommand(Guid ChallengeId) : IRequest<Result>;
}
