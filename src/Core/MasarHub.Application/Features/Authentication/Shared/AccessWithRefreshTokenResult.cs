namespace MasarHub.Application.Features.Authentication.Shared
{
    public record AccessWithRefreshTokenResult(AccessTokenResponse AccessTokenResponse, RefreshTokenResult RefreshTokenResult);
}
