using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Shared;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Token.RefreshToken
{
    public record RefreshTokenCommand(string? RefreshToken, string? IpAddress)
        : IRequest<Result<AccessWithRefreshTokenResult>>;
}
