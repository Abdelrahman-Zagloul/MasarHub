using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Password.ResetPassword
{
    public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result>;

}
