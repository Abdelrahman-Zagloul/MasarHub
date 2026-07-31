using System.Text.Json.Serialization;
using MasarHub.Application.Features.Accounts.Queries.GetCurrentUser;

namespace MasarHub.Application.Features.Accounts.Queries.GetAllInstructors
{
    public sealed record InstructorAccountResponse
    (
        Guid Id,
        string FullName,
        string? UserName,
        string Email,
        string? PhoneNumber,
        string Gender,
        bool EmailConfirmed,
        bool IsLocked,
        string? ProfileImagePublicId,
        string Headline,
        string? Bio,
        string? Company,
        string VerificationStatus,
        string? RejectionReason,
        DateTimeOffset CreatedAt,
        Guid? AdminId,
        [property: JsonIgnore] Guid InstructorProfileId
    )
    {
        public string? ProfileImageUrl { get; set; }
        public List<SocialLinkResponse> SocialLinks { get; set; } = [];
    }
}
