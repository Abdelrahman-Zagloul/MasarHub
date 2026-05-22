using MasarHub.Application.Abstractions.Identity;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Application.Features.Authentication.Commands.Account.Login;
using MasarHub.Application.Features.Authentication.Commands.Email.ResendConfirmEmail;
using MasarHub.Application.Features.Authentication.Commands.Password.ChangePassword;
using MasarHub.Application.Features.Authentication.Commands.Password.ForgetPassword;
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
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                UserName = email,
                PhoneNumber = phoneNumber,
                Gender = gender,
                LockoutEnabled = true,
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
        public async Task<Result<AuthenticateUserResult>> LoginAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return Error.Unauthorized("auth.invalid_credentials");

            if (!user.EmailConfirmed)
                return Error.Forbidden("auth.email_not_confirmed");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
            if (signInResult.IsLockedOut)
                return Error.Forbidden("auth.account_locked");

            if (!signInResult.Succeeded)
                return Error.Unauthorized("auth.invalid_credentials");

            if (user.TwoFactorEnabled && user.PreferredTwoFactorProvider.HasValue)
                return new AuthenticateUserResult(true, new TokenUser(user.Id, user.FullName, user.Email!, []), user.PreferredTwoFactorProvider);

            var roles = await _userManager.GetRolesAsync(user);
            return new AuthenticateUserResult(false, new TokenUser(user.Id, user.FullName, user.Email!, roles), user.PreferredTwoFactorProvider);
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

        public async Task<Result<TokenUser>> ConfirmEmailAsync(string email, string token, CancellationToken ct = default)
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
            return new TokenUser(user.Id, user.FullName, user.Email!, roles);
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
            return new TokenUser(userId, user.FullName, user.Email!, roles);
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
