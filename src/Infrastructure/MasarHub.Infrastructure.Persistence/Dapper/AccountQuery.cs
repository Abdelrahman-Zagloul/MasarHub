using Dapper;
using MasarHub.Application.Abstractions.Persistence.Queries;
using MasarHub.Application.Common.Pagination;
using MasarHub.Application.Features.Accounts.Queries.GetAllAccounts;
using MasarHub.Application.Features.Accounts.Queries.GetAllInstructors;
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

        public async Task<PagedResult<AccountResponse>> GetAllAsync(GetAllAccountsQuery query, CancellationToken ct)
        {
            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                conditions.Add("(u.FullName LIKE @SearchTerm)");
                parameters.Add("SearchTerm", $"%{query.SearchTerm}%");
            }

            if (query.Role.HasValue)
            {
                conditions.Add(@"EXISTS (
                    SELECT 1
                    FROM [identity].[UserRoles] ur
                    INNER JOIN [identity].[Roles] r ON ur.RoleId = r.Id
                    WHERE ur.UserId = u.Id AND r.Name = @Role
                )");
                parameters.Add("Role", query.Role.Value.ToString());
            }

            if (query.EmailConfirmed.HasValue)
            {
                conditions.Add("u.EmailConfirmed = @EmailConfirmed");
                parameters.Add("EmailConfirmed", query.EmailConfirmed.Value);
            }

            if (query.TwoFactorEnabled.HasValue)
            {
                conditions.Add("u.TwoFactorEnabled = @TwoFactorEnabled");
                parameters.Add("TwoFactorEnabled", query.TwoFactorEnabled.Value);
            }

            if (query.IsLocked.HasValue)
            {
                conditions.Add("(u.LockoutEnd > SYSUTCDATETIME()) = @IsLocked");
                parameters.Add("IsLocked", query.IsLocked.Value);
            }

            string whereConditions = conditions.Count > 0
                ? "WHERE " + string.Join(" AND ", conditions)
                : string.Empty;

            string sql = $@"
                -- Get total count for pagination
                SELECT COUNT(1)
                FROM [identity].[Users] u
                {whereConditions};

                -- Get paginated results
                SELECT
                    u.Id,
                    u.FullName,
                    u.UserName,
                    u.Email,
                    u.PhoneNumber,
                    u.Gender,
                    u.EmailConfirmed,
                    u.PhoneNumberConfirmed,
                    u.TwoFactorEnabled,
                    u.PreferredTwoFactorProvider,
                    CASE WHEN u.LockoutEnd > SYSUTCDATETIME() THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsLocked,
                    u.ProfileImagePublicId
                FROM [identity].[Users] u
                {whereConditions}
                ORDER BY u.FullName, u.Id
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                -- Get roles for the current page
                SELECT ur.UserId, r.Name
                FROM [identity].[UserRoles] ur
                INNER JOIN [identity].[Roles] r ON ur.RoleId = r.Id
                WHERE ur.UserId IN (
                    SELECT u.Id
                    FROM [identity].[Users] u
                    {whereConditions}
                    ORDER BY u.FullName, u.Id
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                );";

            int offset = (query.PageNumber - 1) * query.PageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                new CommandDefinition(sql, parameters, cancellationToken: ct));

            var totalCount = await multi.ReadFirstAsync<int>();
            var accounts = (await multi.ReadAsync<AccountResponse>()).ToList();
            var roles = await multi.ReadAsync<UserRoleRow>();

            var rolesLookup = roles
                .GroupBy(r => r.UserId)
                .ToDictionary(g => g.Key, g => g.Select(r => r.Name).ToArray());

            foreach (var account in accounts)
            {
                if (rolesLookup.TryGetValue(account.Id, out var accountRoles))
                    account.Roles = accountRoles;
            }

            return new PagedResult<AccountResponse>(accounts, totalCount);
        }

        public async Task<PagedResult<InstructorAccountResponse>> GetAllInstructorsAsync(GetAllInstructorsQuery query, CancellationToken ct)
        {
            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                conditions.Add("(u.FullName LIKE @SearchTerm OR u.UserName LIKE @SearchTerm OR u.Email LIKE @SearchTerm)");
                parameters.Add("SearchTerm", $"%{query.SearchTerm}%");
            }

            if (query.VerificationStatus.HasValue)
            {
                conditions.Add("ip.VerificationStatus = @VerificationStatus");
                parameters.Add("VerificationStatus", query.VerificationStatus.Value.ToString());
            }

            string whereConditions = conditions.Count > 0
                ? "WHERE " + string.Join(" AND ", conditions)
                : string.Empty;

            string sql = $@"
                -- Get total count for pagination
                SELECT COUNT(1)
                FROM [users].[InstructorProfiles] ip
                INNER JOIN [identity].[Users] u ON u.Id = ip.UserId
                {whereConditions};

                -- Get paginated results
                SELECT
                    u.Id,
                    u.FullName,
                    u.UserName,
                    u.Email,
                    u.PhoneNumber,
                    u.Gender,
                    u.EmailConfirmed,
                    CASE WHEN u.LockoutEnd > SYSUTCDATETIME() THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsLocked,
                    u.ProfileImagePublicId,
                    ip.Headline,
                    ip.Bio,
                    ip.Company,
                    ip.VerificationStatus,
                    ip.RejectionReason,
                    ip.CreatedAt,
                    ip.AdminId,
                    ip.Id AS InstructorProfileId
                FROM [users].[InstructorProfiles] ip
                INNER JOIN [identity].[Users] u ON u.Id = ip.UserId
                {whereConditions}
                ORDER BY ip.CreatedAt DESC, u.Id
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                -- Get social links for the current page
                SELECT sl.Platform, sl.Url, sl.InstructorProfileId
                FROM [users].[InstructorSocialLinks] sl
                WHERE sl.InstructorProfileId IN (
                    SELECT ip.Id
                    FROM [users].[InstructorProfiles] ip
                    INNER JOIN [identity].[Users] u ON u.Id = ip.UserId
                    {whereConditions}
                    ORDER BY ip.CreatedAt DESC, u.Id
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                );";

            int offset = (query.PageNumber - 1) * query.PageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                new CommandDefinition(sql, parameters, cancellationToken: ct));

            var totalCount = await multi.ReadFirstAsync<int>();
            var instructors = (await multi.ReadAsync<InstructorAccountResponse>()).ToList();
            var socialLinks = await multi.ReadAsync<InstructorSocialLinkRow>();

            var linksLookup = socialLinks
                .GroupBy(l => l.InstructorProfileId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(l => new SocialLinkResponse(l.Platform, l.Url)).ToList());

            foreach (var instructor in instructors)
            {
                if (linksLookup.TryGetValue(instructor.InstructorProfileId, out var links))
                    instructor.SocialLinks = links;
            }

            return new PagedResult<InstructorAccountResponse>(instructors, totalCount);
        }

        public async Task<CurrentUserResponse?> GetAccountByIdAsync(Guid userId, CancellationToken ct)
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
                    CASE WHEN u.LockoutEnd > SYSUTCDATETIME() THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsLocked,
                    ip.Id AS InstructorProfileId,
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
                WHERE InstructorProfileId IN (
                    SELECT Id FROM [users].[InstructorProfiles] WHERE UserId = @UserId
                );";

            using var connection = _connectionFactory.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                new CommandDefinition(sql, new { UserId = userId }, cancellationToken: ct));

            var user = await multi.ReadFirstOrDefaultAsync();
            if (user is null)
                return null;

            var roles = (await multi.ReadAsync<string>()).ToArray();

            var account = new CurrentUserResponse
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

            if (user.InstructorProfileId is not null)
            {
                var socialLinks = (await multi.ReadAsync<SocialLinkResponse>()).AsList();

                account.InstructorProfile = new InstructorProfileInfo
                {
                    Headline = user.Headline,
                    Bio = user.Bio,
                    Company = user.Company,
                    VerificationStatus = Enum.TryParse<VerificationStatus>((string)user.VerificationStatus, out var status) ? status : VerificationStatus.Pending,
                    RejectionReason = user.RejectionReason,
                    SocialLinks = socialLinks
                };
            }

            return account;
        }

        private sealed record UserRoleRow(Guid UserId, string Name);
        private sealed record InstructorSocialLinkRow(string Platform, string Url, Guid InstructorProfileId);
    }
}
