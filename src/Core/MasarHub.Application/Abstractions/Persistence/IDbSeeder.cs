using MasarHub.Application.Common.DI;

namespace MasarHub.Application.Abstractions.Persistence
{
    public interface IDbSeeder : IScopedService
    {
        byte Order { get; }
        Task SeedAsync();
    }
}
