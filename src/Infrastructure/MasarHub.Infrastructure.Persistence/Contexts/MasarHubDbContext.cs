using MasarHub.Infrastructure.Persistence.Configurations.Identity;
using MasarHub.Infrastructure.Persistence.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MasarHub.Infrastructure.Persistence.Contexts
{
    internal class MasarHubDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public MasarHubDbContext(DbContextOptions<MasarHubDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(IPersistenceAssemblyMarker).Assembly);
            builder.ApplyIdentitySchema();
        }
    }
}
