using MasarHub.Application.Common.DI;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Features.Authentication.Commands.ChangePassword;
using MasarHub.Application.Features.Authentication.Commands.ConfirmEmail;
using MasarHub.Application.Features.Authentication.Commands.ForgetPassword;
using MasarHub.Application.Features.Authentication.Commands.ResendConfirmEmail;
using MasarHub.Application.Features.Authentication.Shared;
using MasarHub.Domain.Modules.Profiles;

namespace MasarHub.Application.Abstractions.Identity
{
    public interface IAuthService : IScopedService
    {
        Task<Result<RegisterUserResult>> RegisterUserAsync(
            string fullName,
            string email,
            string password,
            string phoneNumber,
            Gender gender,
            UserRole role,
            CancellationToken ct = default);

        Task<Result<ConfirmEmailTokenResult>> GenerateEmailTokenAsync(string email);
        Task<Result<ConfirmedEmailResult>> ConfirmEmailAsync(string email, string token, CancellationToken ct = default);
        Task<Result> DeleteUserAsync(Guid userId);
        Task<Result<TokenUser>> GetUserAsync(Guid userId);
        Task<Result<PasswordChangedResult>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<Result<ForgetPasswordResult>> ForgetPasswordAsync(string email);
        Task<Result<PasswordChangedResult>> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
