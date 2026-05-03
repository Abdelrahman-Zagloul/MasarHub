using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Infrastructure.Persistence.Identity
{
    public sealed class RefreshToken : BaseEntity
    {
        public string TokenHash { get; private set; } = null!;
        public DateTimeOffset ExpiresAt { get; private set; }
        public DateTimeOffset? RevokedAt { get; private set; }
        public Guid? ReplacedByRefreshTokenId { get; private set; }
        public string? CreatedByIp { get; private set; }
        public string? RevokedByIp { get; private set; }
        public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
        public bool IsActive => RevokedAt == null && !IsExpired;
        public Guid UserId { get; private set; }

        private RefreshToken() { }

        private RefreshToken(Guid userId, string tokenHash, DateTimeOffset expiresAt, string? createdByIp)
        {
            UserId = userId;
            TokenHash = tokenHash;
            ExpiresAt = expiresAt;
            CreatedByIp = createdByIp;
        }

        public static Result<RefreshToken> Create(Guid userId, string tokenHash, DateTimeOffset expiresAt, string? createdByIp)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(userId, nameof(userId)),
                Guard.AgainstNullOrWhiteSpace(tokenHash, nameof(tokenHash))
            );

            if (error is not null)
                return error;

            if (expiresAt <= DateTimeOffset.UtcNow)
                return DomainError.Invalid(nameof(expiresAt));

            return new RefreshToken(userId, tokenHash, expiresAt, createdByIp);
        }

        public Result Revoke(string? revokedByIp, Guid? replacedByRefreshTokenId = null)
        {
            if (RevokedAt != null)
                return Result.Success();

            RevokedAt = DateTimeOffset.UtcNow;
            RevokedByIp = revokedByIp;
            ReplacedByRefreshTokenId = replacedByRefreshTokenId;
            MarkAsUpdated();

            return Result.Success();
        }
    }
}
