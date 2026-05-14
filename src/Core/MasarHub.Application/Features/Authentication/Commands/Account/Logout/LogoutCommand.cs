using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Account.Logout
{
    public record LogoutCommand(Guid UserId, string? IpAddress) : IRequest<Result>;
}
