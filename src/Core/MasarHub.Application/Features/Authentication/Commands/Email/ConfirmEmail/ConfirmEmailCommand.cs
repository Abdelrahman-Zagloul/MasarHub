using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Email.ConfirmEmail
{
    public sealed record ConfirmEmailCommand(string Email, string Token, string? IpAddress) : IRequest<Result<AccessWithRefreshTokenResult>>;

    public sealed record ConfirmEmailRequest(string Email, string Token);
}
