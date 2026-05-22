using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Account.ExternalLogin;
using MasarHub.Application.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace MasarHub.Infrastructure.Identity.ExternalLogin.Providers
{
    public sealed class LinkedInAuthProvider : IExternalAuthProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ExternalAuthSettings _settings;

        public ExternalLoginProvider Provider => ExternalLoginProvider.LinkedIn;
        public LinkedInAuthProvider(HttpClient httpClient, IOptions<ExternalAuthSettings> settings)
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

                using var request = new HttpRequestMessage(HttpMethod.Get, _settings.LinkedIn.UserInfoUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return Error.Unauthorized("auth.external_token_invalid");

                var user = await response.Content.ReadFromJsonAsync<LinkedInUserResponse>(cancellationToken);
                if (user is null || string.IsNullOrWhiteSpace(user.Sub))
                    return Error.Unauthorized("auth.external_token_invalid");

                return new ExternalUserInfo
                (
                    user.Email,
                    user.Name,
                    user.Sub,
                    ExternalLoginProvider.LinkedIn
                );
            }
            catch
            {
                return Error.Unauthorized("auth.external_token_invalid");
            }
        }

        private async Task<string?> ExchangeCodeAsync(string code, CancellationToken cancellationToken)
        {
            var content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["grant_type"] = "authorization_code",
                    ["code"] = code,
                    ["client_id"] = _settings.LinkedIn.ClientId,
                    ["client_secret"] = _settings.LinkedIn.ClientSecret,
                    ["redirect_uri"] = _settings.LinkedIn.RedirectUrl,
                });

            var response = await _httpClient.PostAsync(_settings.LinkedIn.TokenUrl, content, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<LinkedInTokenResponse>(cancellationToken);

            return result?.AccessToken;
        }

        private sealed record LinkedInTokenResponse
        (
            [property: JsonPropertyName("access_token")] string AccessToken
        );
        private sealed record LinkedInUserResponse
        (
            string Sub,
            string Name,
            string Email
        );
    }
}