using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Seeding
{
    public interface IDbSeeder : IScopedService
    {
        byte Order { get; }
        Task SeedAsync();
    }
}
