namespace MasarHub.Application.Features.Authentication.Commands.ForgetPassword
{
    public sealed record ForgetPasswordResult(string FullName, string Email, string Token);
}
