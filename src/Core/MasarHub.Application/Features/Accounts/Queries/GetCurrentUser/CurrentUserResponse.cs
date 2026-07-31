using MasarHub.Domain.Modules.Profiles;
using System.Text.Json.Serialization;

namespace MasarHub.Application.Features.Accounts.Queries.GetCurrentUser
{
    public sealed record CurrentUserResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? UserName { get; set; }
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Gender { get; set; } = null!;
        public string? ProfileImageUrl { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? PreferredTwoFactorProvider { get; set; }
        public bool IsLocked { get; set; }
        public string[] Roles { get; set; } = [];
        public InstructorProfileInfo? InstructorProfile { get; set; }

        [JsonIgnore]
        public string? ProfileImagePublicId { get; set; }
    }

    public sealed record InstructorProfileInfo
    {
        public string Headline { get; set; } = null!;
        public string? Bio { get; set; }
        public string? Company { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public string? RejectionReason { get; set; }
        public List<SocialLinkResponse> SocialLinks { get; set; } = [];
    }

    public sealed record SocialLinkResponse(string Platform, string Url);
}
