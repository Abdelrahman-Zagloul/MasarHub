using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Features.Accounts.Queries.GetCurrentUser;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Infrastructure.Persistence.Dapper
{
    public sealed class AccountQuery : IAccountQuery
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AccountQuery(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<VerificationStatus?> GetInstructorStatusAsync(Guid userId, CancellationToken ct)
        {
            const string sql = @"
                SELECT VerificationStatus
                FROM [users].[InstructorProfiles]
                WHERE UserId = @UserId;";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { UserId = userId }, cancellationToken: ct);

            var statusString = await connection.QueryFirstOrDefaultAsync<string>(command);
            if (statusString is null)
                return null;

            return Enum.TryParse<VerificationStatus>(statusString, out var status) ? status : null;
        }

        public async Task<CurrentUserResponse?> GetUserProfileAsync(Guid userId, CancellationToken ct)
        {
            const string sql = @"
                SELECT
                    u.Id,
                    u.FullName,
                    u.UserName,
                    u.Email,
                    u.PhoneNumber,
                    u.Gender,
                    u.ProfileImagePublicId,
                    u.EmailConfirmed,
                    u.PhoneNumberConfirmed,
                    u.TwoFactorEnabled,
                    u.PreferredTwoFactorProvider,
                    u.LockoutEnd,
                    CASE WHEN u.LockoutEnd > SYSUTCDATETIME() THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsLocked
                FROM [identity].[Users] u
                WHERE u.Id = @UserId;

                SELECT r.Name
                FROM [identity].[UserRoles] ur
                INNER JOIN [identity].[Roles] r ON ur.RoleId = r.Id
                WHERE ur.UserId = @UserId;";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { UserId = userId }, cancellationToken: ct);

            using var multi = await connection.QueryMultipleAsync(command);

            var user = await multi.ReadFirstOrDefaultAsync();
            if (user is null)
                return null;

            var roles = (await multi.ReadAsync<string>()).ToArray();

            return new CurrentUserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                ProfileImagePublicId = user.ProfileImagePublicId,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                PreferredTwoFactorProvider = user.PreferredTwoFactorProvider,
                IsLocked = user.IsLocked,
                Roles = roles,
                InstructorProfile = null
            };
        }

        public async Task<CurrentUserResponse?> GetInstructorProfileAsync(Guid userId, CancellationToken ct)
        {
            const string sql = @"
                SELECT
                    u.Id,
                    u.FullName,
                    u.UserName,
                    u.Email,
                    u.PhoneNumber,
                    u.Gender,
                    u.ProfileImagePublicId,
                    u.EmailConfirmed,
                    u.PhoneNumberConfirmed,
                    u.TwoFactorEnabled,
                    u.PreferredTwoFactorProvider,
                    u.LockoutEnd,
                    CASE WHEN u.LockoutEnd > SYSUTCDATETIME() THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsLocked,
                    ip.Headline,
                    ip.Bio,
                    ip.Company,
                    ip.VerificationStatus,
                    ip.RejectionReason
                FROM [identity].[Users] u
                LEFT JOIN [users].[InstructorProfiles] ip ON u.Id = ip.UserId
                WHERE u.Id = @UserId;

                SELECT r.Name
                FROM [identity].[UserRoles] ur
                INNER JOIN [identity].[Roles] r ON ur.RoleId = r.Id
                WHERE ur.UserId = @UserId;

                SELECT Platform, Url
                FROM [users].[InstructorSocialLinks]
                WHERE InstructorProfileId = (
                    SELECT Id FROM [users].[InstructorProfiles] WHERE UserId = @UserId
                );";

            using var connection = _connectionFactory.CreateConnection();
            var command = new CommandDefinition(sql, new { UserId = userId }, cancellationToken: ct);

            using var multi = await connection.QueryMultipleAsync(command);

            var user = await multi.ReadFirstOrDefaultAsync();
            if (user is null)
                return null;

            var roles = (await multi.ReadAsync<string>()).ToArray();
            var socialLinks = (await multi.ReadAsync<SocialLinkResponse>()).AsList();

            return new CurrentUserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                ProfileImagePublicId = user.ProfileImagePublicId,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                PreferredTwoFactorProvider = user.PreferredTwoFactorProvider,
                IsLocked = user.IsLocked,
                Roles = roles,
                InstructorProfile = new InstructorProfileInfo
                {
                    Headline = user.Headline,
                    Bio = user.Bio,
                    Company = user.Company,
                    VerificationStatus = Enum.TryParse<VerificationStatus>((string)user.VerificationStatus, out var status) ? status : VerificationStatus.Pending,
                    RejectionReason = user.RejectionReason,
                    SocialLinks = socialLinks
                }
            };
        }
    }
}
