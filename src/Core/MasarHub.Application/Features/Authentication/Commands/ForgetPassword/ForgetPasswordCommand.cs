using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.ForgetPassword
{
    public sealed record ForgetPasswordCommand(string Email) : IRequest<Result>;
}
