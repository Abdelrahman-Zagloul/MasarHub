using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Profiles;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.EnableTwoFactor
{
    public sealed record EnableTwoFactorCommand(TwoFactorProvider Provider) : IRequest<Result>;
}
