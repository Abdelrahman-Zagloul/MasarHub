using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;

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
        //public ApplicationUser User { get; private set; } = null!;

        private RefreshToken() { }
        private RefreshToken(Guid userId, string tokenHash, DateTimeOffset expiresAt, string? createdByIp)
        {
            UserId = Guard.AgainstEmptyGuid(userId, nameof(userId));
            TokenHash = Guard.AgainstNullOrWhiteSpace(tokenHash, nameof(tokenHash));
            CreatedByIp = createdByIp;

            if (expiresAt <= DateTimeOffset.UtcNow)
                throw new InvalidOperationException(ErrorCodes.General.Invalid);

            ExpiresAt = expiresAt;

        }
        public static RefreshToken Create(Guid userId, string tokenHash, DateTimeOffset expiresAt, string? createdByIp)
        {
            return new RefreshToken(userId, tokenHash, expiresAt, createdByIp);
        }

        public void Revoke(string? revokedByIp, Guid? replacedByRefreshTokenId = null)
        {
            if (RevokedAt != null)
                return;

            RevokedAt = DateTimeOffset.UtcNow;
            RevokedByIp = revokedByIp;
            ReplacedByRefreshTokenId = replacedByRefreshTokenId;
            MarkAsUpdated();
        }
    }
}
