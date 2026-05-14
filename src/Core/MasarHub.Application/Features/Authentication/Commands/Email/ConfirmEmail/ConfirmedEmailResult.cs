using MasarHub.Application.Features.Authentication.Shared;

namespace MasarHub.Application.Features.Authentication.Commands.Email.ConfirmEmail
{
    public sealed record ConfirmedEmailResult(TokenUser User, string FullName);
}
