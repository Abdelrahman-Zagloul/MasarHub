using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Password.ChangePassword
{
    public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : IRequest<Result>;
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
}
