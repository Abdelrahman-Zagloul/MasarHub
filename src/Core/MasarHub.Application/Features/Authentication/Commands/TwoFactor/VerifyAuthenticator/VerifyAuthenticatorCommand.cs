using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.VerifyAuthenticator
{
    public sealed record VerifyAuthenticatorCommand(string Code) : IRequest<Result>;
}
