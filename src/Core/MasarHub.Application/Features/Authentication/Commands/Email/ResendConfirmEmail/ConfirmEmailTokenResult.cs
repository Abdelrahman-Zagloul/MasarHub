namespace MasarHub.Application.Features.Authentication.Commands.Email.ResendConfirmEmail
{
    public sealed record ConfirmEmailTokenResult(string FullName, string Email, string Token);
}
