namespace MasarHub.Application.Features.Authentication.Shared
{
    public sealed record TokenUser(Guid Id, string FullName, string Email, IEnumerable<string> Roles);
}
