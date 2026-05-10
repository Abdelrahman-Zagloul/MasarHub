namespace MasarHub.Application.Features.Authentication.Commands.ChangePassword
{
    public sealed record PasswordChangedResult(Guid UserId, string FullName, string Email);
}
