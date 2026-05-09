namespace MasarHub.Application.Features.Authentication.Commands.ResendConfirmEmail
{
    public sealed record ConfirmEmailTokenResult(string FullName, string Email, string Token);
}
