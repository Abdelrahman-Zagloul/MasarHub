namespace MasarHub.Application.Features.Authentication.Shared
{
    public sealed record TokenUser(Guid Id, string Email, IEnumerable<string> Roles);
}
