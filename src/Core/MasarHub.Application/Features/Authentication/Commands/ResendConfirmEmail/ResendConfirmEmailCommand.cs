using MasarHub.Application.Common.Results;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.ResendConfirmEmail
{
    public sealed record ResendConfirmEmailCommand(string Email) : IRequest<Result>;

}
