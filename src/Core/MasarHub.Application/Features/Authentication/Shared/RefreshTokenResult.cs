namespace MasarHub.Application.Features.Authentication.Shared
{
    public record RefreshTokenResult(string RefreshToken, DateTimeOffset ExpiresAt, Guid UserId);
}
