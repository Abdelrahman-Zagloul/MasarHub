using MasarHub.Application.Abstractions.Persistence;
using MasarHub.Application.Common.Constants;
using MasarHub.Application.Settings;
using MasarHub.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MasarHub.Infrastructure.Persistence.SeedData
{
    public class UserSeeder : IDbSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DefaultUsersSettings _settings;
        private readonly ILogger<UserSeeder> _logger;
        public UserSeeder(UserManager<ApplicationUser> userManager, IOptions<DefaultUsersSettings> options, ILogger<UserSeeder> logger)
        {
            _userManager = userManager;
            _settings = options.Value;
            _logger = logger;
        }

        public byte Order => 2;

        public async Task SeedAsync()
        {
            if (!_settings.SeedDefaultUsers)
            {
                _logger.LogInformation("Default user seeding is disabled.");
                return;
            }
            await CreateUserAsync("Admin", _settings.AdminEmail, _settings.AdminPassword, Roles.Admin);
            await CreateUserAsync("Instructor", _settings.InstructorEmail, _settings.InstructorPassword, Roles.Instructor);
            await CreateUserAsync("Student", _settings.StudentEmail, _settings.StudentPassword, Roles.Student);
        }

        private async Task CreateUserAsync(string name, string email, string password, string role)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                _logger.LogInformation("User '{Email}' already exists.", email);
                return;
            }

            var user = new ApplicationUser
            {
                FullName = name,
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                _logger.LogError("Failed to create user '{Email}': {Errors}",
                    email,
                    string.Join(", ", result.Errors.Select(e => e.Description)));

                return;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                _logger.LogError("User '{Email}' created but failed to assign role '{Role}': {Errors}",
                    email,
                    role,
                    string.Join(", ", roleResult.Errors.Select(e => e.Description)));

                return;
            }

            _logger.LogInformation("User '{Email}' created successfully with role '{Role}'.", email, role);
        }
    }
}
