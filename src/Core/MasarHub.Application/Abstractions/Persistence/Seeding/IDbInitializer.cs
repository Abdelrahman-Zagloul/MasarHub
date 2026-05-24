using MasarHub.Application.Common.DependencyInjection;

namespace MasarHub.Application.Abstractions.Persistence.Seeding
{
    public interface IDbInitializer : IScopedService
    {
        Task InitializeAsync();
    }
}
