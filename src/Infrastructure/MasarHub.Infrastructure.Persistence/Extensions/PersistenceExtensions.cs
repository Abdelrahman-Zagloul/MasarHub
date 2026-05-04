using MasarHub.Application.Abstractions.Persistence;
using MasarHub.Infrastructure.Persistence.Contexts;
using MasarHub.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MasarHub.Infrastructure.Persistence.Extensions
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MasarHubDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            return services;
        }
    }
}
