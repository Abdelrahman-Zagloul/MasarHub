using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.VerifyPassword
{
    public sealed class VerifyPasswordCommandHandler : IRequestHandler<VerifyPasswordCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;
        public VerifyPasswordCommandHandler(IAuthService authService, ICurrentUserService currentUserService)
        {
            _authService = authService;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(VerifyPasswordCommand request, CancellationToken cancellationToken)
        {
            return await _authService.VerifyPasswordAsync(_currentUserService.UserId, request.Password);
        }
    }
}
