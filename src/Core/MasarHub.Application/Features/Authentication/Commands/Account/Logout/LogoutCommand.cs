using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Account.Logout
{
    public sealed record LogoutCommand(Guid UserId, string? IpAddress) : IRequest<Result>;
}
