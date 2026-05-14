using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Account.Login
{
    public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginResult>>;

}