using MasarHub.Application.Abstractions.Persistence;
using MasarHub.Infrastructure.Persistence.Contexts;

namespace MasarHub.Infrastructure.Persistence.Repositories
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly MasarHubDbContext _context;

        public UnitOfWork(MasarHubDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }
    }
}
