using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Email.ResendConfirmEmail
{
    public sealed record ResendConfirmEmailCommand(string Email) : IRequest<Result>;

}
