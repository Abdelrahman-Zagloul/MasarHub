namespace MasarHub.Application.Features.Authentication.Shared
{
    public sealed record RegisterUserResult(string EmailVerificationToken, Guid UserId);
}
