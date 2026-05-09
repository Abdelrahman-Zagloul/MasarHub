using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Logout
{
    public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IRefreshTokenService _refreshTokenService;

        public LogoutCommandHandler(IRefreshTokenService refreshTokenService)
        {
            _refreshTokenService = refreshTokenService;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _refreshTokenService.RevokeAllAsync(request.UserId, request.IpAddress, cancellationToken);
            return Result.Success();
        }
    }
}
