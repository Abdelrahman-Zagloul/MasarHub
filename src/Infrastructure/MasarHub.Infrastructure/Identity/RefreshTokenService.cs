using Dapper;
using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Abstractions.Persistence;
using MasarHub.Application.Abstractions.Queries;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Application.Settings;
using MasarHub.Infrastructure.Persistence.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace MasarHub.Infrastructure.Identity
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RefreshTokenSettings _settings;
        private readonly IRepository<RefreshToken> _refreshTokenRepo;
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public RefreshTokenService(IUnitOfWork unitOfWork, IOptions<RefreshTokenSettings> options, IRepository<RefreshToken> refreshTokenRepo, IDbConnectionFactory dbConnectionFactory)
        {
            _unitOfWork = unitOfWork;
            _settings = options.Value;
            _refreshTokenRepo = refreshTokenRepo;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<Result<RefreshTokenResult>> CreateAsync(TokenUser user, string? ipAddress, CancellationToken ct)
        {
            var rawToken = GenerateRefreshToken();
            var tokenHash = HashRefreshToken(rawToken);
            var expiresAt = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenLifetimeDays);

            var newRefreshToken = RefreshToken.Create(user.Id, tokenHash, expiresAt, ipAddress);

            await _refreshTokenRepo.AddAsync(newRefreshToken, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return new RefreshTokenResult(rawToken, newRefreshToken.ExpiresAt, user.Id);
        }
        public async Task<Result<RefreshTokenResult>> RotateAsync(string refreshToken, string? ipAddress, CancellationToken ct)
        {
            var tokenHash = HashRefreshToken(refreshToken);
            var existingToken = await _refreshTokenRepo.GetAsync(x => x.TokenHash == tokenHash, ct);

            if (existingToken is null)
                return Error.BadRequest("token.invalid");

            if (!existingToken.IsActive)
            {
                if (existingToken.ReplacedByRefreshTokenId != null)
                    await RevokeAllAsync(existingToken.UserId, ipAddress, ct);
                return Error.BadRequest("token.invalid");
            }


            var newRawToken = GenerateRefreshToken();
            var newTokenHash = HashRefreshToken(newRawToken);
            var expiresAt = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenLifetimeDays);
            var newRefreshToken = RefreshToken.Create(existingToken.UserId, newTokenHash, expiresAt, ipAddress);


            await _refreshTokenRepo.AddAsync(newRefreshToken, ct);
            existingToken.Revoke(ipAddress, newRefreshToken.Id);
            await _unitOfWork.SaveChangesAsync(ct);

            return new RefreshTokenResult(newRawToken, newRefreshToken.ExpiresAt, existingToken.UserId);
        }
        public async Task<Result> RevokeAsync(string refreshToken, string? ipAddress, CancellationToken ct)
        {
            var tokenHash = HashRefreshToken(refreshToken);
            var token = await _refreshTokenRepo.GetAsync(x => x.TokenHash == tokenHash, ct);

            if (token is null || !token.IsActive)
                return Error.BadRequest("token.invalid");

            token.Revoke(ipAddress);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Success();
        }
        public async Task RevokeAllAsync(Guid userId, string? ipAddress, CancellationToken ct)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            string sql = @"
                UPDATE [identity].RefreshTokens 
                SET 
                    RevokedAt = @Now,
                    RevokedByIp = @IpAddress,
                    ReplacedByRefreshTokenId = NULL,
                    UpdatedAt = @Now
                WHERE 
                    UserId = @UserId AND
                    RevokedAt IS NULL AND 
                    ExpiresAt > @Now
                ";

            var command = new CommandDefinition(sql, new
            {
                UserId = userId,
                IpAddress = ipAddress,
                Now = DateTimeOffset.UtcNow
            }, cancellationToken: ct);

            await connection.ExecuteAsync(command);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
        private string HashRefreshToken(string refreshToken)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_settings.RefreshTokenHashKey);
            using var hmac = new HMACSHA256(keyBytes);
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(refreshToken)));
        }
    }
}

