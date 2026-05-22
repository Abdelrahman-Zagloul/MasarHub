using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.GenerateRecoveryCodes
{
    public sealed record GenerateRecoveryCodesCommand(Guid UserId) : IRequest<Result<IEnumerable<string>>>;
}
