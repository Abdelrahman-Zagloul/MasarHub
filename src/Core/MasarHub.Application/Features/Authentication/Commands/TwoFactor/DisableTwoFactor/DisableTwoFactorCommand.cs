using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.DisableTwoFactor
{
    public sealed record DisableTwoFactorCommand(Guid UserId) : IRequest<Result>;
}
