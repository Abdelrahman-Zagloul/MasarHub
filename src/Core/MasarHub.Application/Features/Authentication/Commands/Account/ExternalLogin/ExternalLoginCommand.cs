using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin
{
    public sealed record ExternalLoginCommand(ExternalLoginProvider Provider, string Token) : IRequest<Result<AccessWithRefreshTokenResult>>;
}
