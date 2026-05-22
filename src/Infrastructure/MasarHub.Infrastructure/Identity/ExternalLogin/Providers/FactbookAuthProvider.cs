using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin;
using MasarHub.Application.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace MasarHub.Infrastructure.Identity.ExternalLogin.Providers
{
    public sealed class FacebookAuthProvider : IExternalAuthProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ExternalAuthSettings _settings;

        public ExternalLoginProvider Provider => ExternalLoginProvider.Facebook;

        public FacebookAuthProvider(HttpClient httpClient, IOptions<ExternalAuthSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<Result<ExternalUserInfo>> VerifyAsync(string token, CancellationToken cancellationToken = default)
        {
            try
            {
                // token is authorization code
                var accessToken = await ExchangeCodeAsync(token, cancellationToken);
                if (string.IsNullOrWhiteSpace(accessToken))
                    return Error.Unauthorized("auth.external_token_invalid");

                var url = $"{_settings.Facebook.UserInfoUrl}" + $"?fields=id,name,email" + $"&access_token={accessToken}";
                var response = await _httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return Error.Unauthorized("auth.external_token_invalid");

                var user = await response.Content.ReadFromJsonAsync<FacebookUserResponse>(cancellationToken);

                if (user is null || string.IsNullOrWhiteSpace(user.Id))
                    return Error.Unauthorized("auth.external_token_invalid");

                if (string.IsNullOrWhiteSpace(user.Email))
                    return Error.Unauthorized("auth.external_email_not_available");

                return new ExternalUserInfo
                (
                    user.Email,
                    user.Name,
                    user.Id,
                    ExternalLoginProvider.Facebook
                );
            }
            catch
            {
                return Error.Unauthorized("auth.external_token_invalid");
            }
        }

        private async Task<string?> ExchangeCodeAsync(string code, CancellationToken cancellationToken)
        {
            var url =
                $"{_settings.Facebook.TokenUrl}" +
                $"?client_id={_settings.Facebook.ClientId}" +
                $"&client_secret={_settings.Facebook.ClientSecret}" +
                $"&redirect_uri={Uri.EscapeDataString(_settings.Facebook.RedirectUri)}" +
                $"&code={code}";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<FacebookTokenResponse>(cancellationToken);
            return result?.AccessToken;
        }

        private sealed record FacebookTokenResponse
        (
            [property: JsonPropertyName("access_token")] string AccessToken
        );

        private sealed record FacebookUserResponse
        (
            string Id,
            string Name,
            string? Email
        );
    }
}
