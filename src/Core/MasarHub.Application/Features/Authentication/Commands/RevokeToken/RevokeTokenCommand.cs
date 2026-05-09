using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.RevokeToken
{
    public record RevokeTokenCommand(string? RefreshToken, string? IpAddress) : IRequest<Result>;
}
