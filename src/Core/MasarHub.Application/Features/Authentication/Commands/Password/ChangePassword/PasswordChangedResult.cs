namespace MasarHub.Application.Features.Authentication.Commands.Password.ChangePassword
{
    public sealed record PasswordChangedResult(Guid UserId, string FullName, string Email);
}
