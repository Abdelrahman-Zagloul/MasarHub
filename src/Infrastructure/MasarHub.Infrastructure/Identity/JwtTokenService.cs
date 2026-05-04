using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MasarHub.Infrastructure.Identity
{
    internal class JwtTokenService : ITokenService
    {
        private readonly JWTSettings _settings;

        public JwtTokenService(IOptions<JWTSettings> options)
        {
            _settings = options.Value;
        }

        public async Task<AccessTokenResponse> GenerateTokenAsync(TokenUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.DurationInMinutes),
                signingCredentials: credentials
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return new AccessTokenResponse(jwtToken, token.ValidTo, user.Id, user.Roles.ToArray());
        }
    }
}
