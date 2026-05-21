using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.TwoFactor.SetupAuthenticator
{
    public sealed record SetupAuthenticatorCommand(Guid UserId) : IRequest<Result<SetupAuthenticatorResult>>;
    public sealed record SetupAuthenticatorResult(string SharedKey, string AuthenticatorUri);

}
