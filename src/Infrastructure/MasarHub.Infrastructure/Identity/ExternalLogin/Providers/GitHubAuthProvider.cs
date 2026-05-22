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
    public sealed class GitHubAuthProvider : IExternalAuthProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ExternalAuthSettings _settings;
        public ExternalLoginProvider Provider => ExternalLoginProvider.GitHub;
        public GitHubAuthProvider(HttpClient httpClient, IOptions<ExternalAuthSettings> settings)
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

                using var request = CreateRequest(HttpMethod.Get, _settings.GitHub.UserUrl, accessToken);
                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return Error.Unauthorized("auth.external_token_invalid");

                var user = await response.Content.ReadFromJsonAsync<GitHubUserResponse>(cancellationToken);
                if (user is null || user.Id == 0)
                    return Error.Unauthorized("auth.external_token_invalid");

                var email = user.Email;
                if (string.IsNullOrWhiteSpace(email))
                    email = await GetPrimaryEmailAsync(accessToken, cancellationToken);

                if (string.IsNullOrWhiteSpace(email))
                    return Error.Unauthorized("auth.external_email_not_available");

                var fullName = string.IsNullOrWhiteSpace(user.Name)
                    ? user.Login
                    : user.Name;

                return new ExternalUserInfo
                (
                    email,
                    fullName,
                    user.Id.ToString(),
                    ExternalLoginProvider.GitHub
                );
            }
            catch
            {
                return Error.Unauthorized("auth.external_token_invalid");
            }
        }

        private async Task<string?> ExchangeCodeAsync(string code, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, _settings.GitHub.TokenUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["client_id"] = _settings.GitHub.ClientId,
                    ["client_secret"] = _settings.GitHub.ClientSecret,
                    ["code"] = code
                });


            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<GitHubTokenResponse>(cancellationToken);
            return result?.AccessToken;
        }

        private async Task<string?> GetPrimaryEmailAsync(string accessToken, CancellationToken cancellationToken)
        {
            using var request = CreateRequest(HttpMethod.Get, _settings.GitHub.UserEmailsUrl, accessToken);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            var emails = await response.Content.ReadFromJsonAsync<List<GitHubEmailResponse>>(cancellationToken);
            return emails?
                .Where(x => x.Verified)
                .OrderByDescending(x => x.Primary)
                .Select(x => x.Email)
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        }

        private static HttpRequestMessage CreateRequest(HttpMethod method, string url, string accessToken)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.UserAgent.ParseAdd("MasarHub");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");

            return request;
        }

        private sealed record GitHubTokenResponse
        (
            [property: JsonPropertyName("access_token")] string AccessToken
        );

        private sealed record GitHubUserResponse
        (
            long Id,
            string Login,
            string? Name,
            string? Email
        );

        private sealed record GitHubEmailResponse
        (
            string Email,
            bool Primary,
            bool Verified
        );
    }
}