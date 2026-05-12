using MasarHub.Application.Common.DI;

namespace MasarHub.Application.Abstractions.Identity
{
    public interface ICurrentUserService : IScopedService
    {
        Guid? UserId { get; }
        string? Email { get; }
        string? IpAddress { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
        IEnumerable<string> Roles { get; }
    }
}
