using MasarHub.Application.Common.Results;
using MasarHub.Domain.Modules.Profiles;
using MediatR;

namespace MasarHub.Application.Features.Authentication.Commands.Account.RegisterInstructor
{
    public sealed record RegisterInstructorCommand
    (
        string FullName,
        string Email,
        string PhoneNumber,
        Gender Gender,
        string Password,
        string Headline,
        string? Bio,
        string? Company,
        List<SocialLinkRequest> SocialLinks
    ) : IRequest<Result>;

    public sealed record SocialLinkRequest(string PlatformName, string Url);
}
