namespace MasarHub.Application.Features.Authentication.Shared
{
    public sealed record AccessTokenResponse
    (
        string AccessToken,
        DateTime ExpiresAt,
        Guid UserId,
        string[] Roles
    );
}
