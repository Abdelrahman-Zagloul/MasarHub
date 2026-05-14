using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Password.VerifyPassword
{
    public sealed record VerifyPasswordCommand(string Password) : IRequest<Result>;
}
