using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;
using MasarHub.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MasarHub.Infrastructure.Identity
{
    public sealed class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result<string>> RegisterUserAsync(
            string fullName,
            string email,
            string password,
            string phoneNumber,
            Gender gender,
            UserRole role,
            CancellationToken ct)
        {
            var emailExists = await _userManager.Users.AnyAsync(u => u.Email == email, ct);
            if (emailExists)
                return Error.Conflict("auth.email.already_exists");

            var user = new ApplicationUser
            {
                FullName = fullName,
                Email = email,
                UserName = Guid.CreateVersion7().ToString(),
                PhoneNumber = phoneNumber,
                Gender = gender,
            };

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                return IdentityErrorsMapper.Map(createResult.Errors);

            var addToRoleResult = await _userManager.AddToRoleAsync(user, role.ToString());
            if (!addToRoleResult.Succeeded)
                return Error.Failure("auth.role_assignment_failed");


            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return token;
        }
        public async Task<Result<string>> GenerateEmailTokenAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Error.NotFound("user.not_found");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return token;
        }
    }
}
