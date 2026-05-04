using MasarHub.Application.Abstractions.Persistence;
using MasarHub.Application.Common.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace MasarHub.Infrastructure.Persistence.SeedData
{
    public class RoleSeeder : IDbSeeder
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<RoleSeeder> _logger;
        public RoleSeeder(RoleManager<IdentityRole<Guid>> roleManager, ILogger<RoleSeeder> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public byte Order => 1;
        public async Task SeedAsync()
        {
            string[] roles = { Roles.Admin, Roles.Instructor, Roles.Student };

            var existingRoles = _roleManager.Roles
                .Select(r => r.Name)
                .ToHashSet();

            foreach (var role in roles)
            {
                if (existingRoles.Contains(role))
                {
                    _logger.LogInformation("Role '{Role}' already exists.", role);
                    continue;
                }

                var result = await _roleManager.CreateAsync(new IdentityRole<Guid>(role));

                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to create role '{Role}': {Errors}",
                        role,
                        string.Join(", ", result.Errors.Select(e => e.Description)));

                    continue;
                }

                _logger.LogInformation("Role '{Role}' created successfully.", role);
            }
        }
    }
}
