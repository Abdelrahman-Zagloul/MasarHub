using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Email.ConfirmEmail
{
    public sealed record ConfirmEmailCommand(string Email, string Token) : IRequest<Result<AccessWithRefreshTokenResult>>;

}
