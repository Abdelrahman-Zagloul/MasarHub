using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.VerifyPassword
{
    public sealed record VerifyPasswordCommand(string Password) : IRequest<Result>;
}
