namespace MasarHub.Application.Features.Accounts.Queries.GetAllAccounts
{
    public sealed record AccountResponse
    (
        Guid Id,
        string FullName,
        string? UserName,
        string Email,
        string? PhoneNumber,
        string Gender,
        bool EmailConfirmed,
        bool PhoneNumberConfirmed,
        bool TwoFactorEnabled,
        string? PreferredTwoFactorProvider,
        bool IsLocked,
        string? ProfileImagePublicId
    )
    {
        public string? ProfileImageUrl { get; set; }
        public string[] Roles { get; set; } = [];
    }
}
