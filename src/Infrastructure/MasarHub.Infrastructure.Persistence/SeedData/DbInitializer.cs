using MasarHub.Application.Abstractions.Persistence;
using MasarHub.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MasarHub.Infrastructure.Persistence.SeedData
{
    public sealed class DbInitializer : IDbInitializer
    {
        private readonly IEnumerable<IDbSeeder> _seeders;
        private readonly MasarHubDbContext _context;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(IEnumerable<IDbSeeder> seeders, MasarHubDbContext context, ILogger<DbInitializer> logger)
        {
            _seeders = seeders;
            _context = context;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Database migrated successfully.");
            }

            foreach (var seeder in _seeders.OrderBy(s => s.Order))
            {
                try
                {
                    _logger.LogInformation("Running seeder: {Seeder}", seeder.GetType().Name);

                    await seeder.SeedAsync();

                    _logger.LogInformation("Seeder {Seeder} completed.", seeder.GetType().Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Seeder {Seeder} failed.", seeder.GetType().Name);
                    throw;
                }
            }
        }
    }
}