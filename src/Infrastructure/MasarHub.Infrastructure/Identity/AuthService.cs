using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.ChangePassword;
using MasarHub.Application.Features.Authentication.Commands.ConfirmEmail;
using MasarHub.Application.Features.Authentication.Commands.ForgetPassword;
using MasarHub.Application.Features.Authentication.Commands.ResendConfirmEmail;
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
        public async Task<Result<RegisterUserResult>> RegisterUserAsync(
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
            return new RegisterUserResult(token, user.Id);
        }
        public async Task<Result<ConfirmEmailTokenResult>> GenerateEmailTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Error.NotFound("user.not_found");
            if (user.EmailConfirmed)
                return Error.Conflict("auth.email.already_confirmed");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return new ConfirmEmailTokenResult(user.FullName, user.Email!, token);
        }

        public async Task<Result<ConfirmedEmailResult>> ConfirmEmailAsync(string email, string token, CancellationToken ct = default)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
            if (user is null)
                return Error.NotFound("user.not_found");

            if (user.EmailConfirmed)
                return Error.Conflict("auth.email.already_confirmed");

            var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
                return Error.BadRequest("auth.email_confirmation_failed");

            var roles = await _userManager.GetRolesAsync(user);
            return new ConfirmedEmailResult(new TokenUser(user.Id, user.Email!, roles), user.FullName);
        }

        public async Task<Result> DeleteUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Error.NotFound("user.not_found");
            await _userManager.DeleteAsync(user);
            return Result.Success();
        }

        public async Task<Result<TokenUser>> GetUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Error.NotFound("user.not_found");

            var roles = await _userManager.GetRolesAsync(user);
            return new TokenUser(userId, user.Email!, roles);
        }

        public async Task<Result<PasswordChangedResult>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Error.NotFound("user.not_found");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return IdentityErrorsMapper.Map(result.Errors);

            return new PasswordChangedResult(userId, user.FullName, user.Email!);
        }

        public async Task<Result<ForgetPasswordResult>> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Error.NotFound("user.not_found");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return new ForgetPasswordResult(user.FullName, email, token);
        }

        public async Task<Result<PasswordChangedResult>> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Error.NotFound("user.not_found");
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
                return IdentityErrorsMapper.Map(result.Errors);

            return new PasswordChangedResult(user.Id, user.FullName, user.Email!);
        }

        public async Task<Result> VerifyPasswordAsync(Guid userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Error.NotFound("user.not_found");

            var valid = await _userManager.CheckPasswordAsync(user, password);
            if (!valid)
                return Error.BadRequest("auth.invalid_password");

            return Result.Success();
        }
    }
}
