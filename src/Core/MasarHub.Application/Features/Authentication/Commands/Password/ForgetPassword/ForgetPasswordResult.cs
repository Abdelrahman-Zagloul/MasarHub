namespace MasarHub.Application.Features.Authentication.Commands.Password.ForgetPassword
{
    public sealed record ForgetPasswordResult(string FullName, string Email, string Token);
}
