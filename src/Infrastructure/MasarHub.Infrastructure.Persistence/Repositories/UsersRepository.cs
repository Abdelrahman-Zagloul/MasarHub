using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using MasarHub.Domain.Modules.Profiles;
using MasarHub.Infrastructure.Persistence.Contexts;

namespace MasarHub.Infrastructure.Persistence.Repositories
{
    public sealed class UsersRepository : IUsersRepository
    {
        private readonly MasarHubDbContext _context;

        public UsersRepository(MasarHubDbContext context)
        {
            _context = context;
        }

        public async Task<Result> UpdateProfileImageAsync(Guid userId, string newPublicId, CancellationToken ct = default)
        {
            var user = await _context.Users.FindAsync(userId, ct);
            if (user == null)
                return Error.NotFound("user.not_found");

            var result = user.UpdateProfileImage(newPublicId);
            if (result.IsFailure)
                return Error.BadRequest(result.Error.Code);

            return Result.Success();
        }

        public async Task<Result> UpdateAccountAsync(Guid userId, string? phoneNumber, Gender? gender, TwoFactorProvider? preferredTwoFactorProvider, CancellationToken ct = default)
        {
            var user = await _context.Users.FindAsync(userId, ct);
            if (user == null)
                return Error.NotFound("user.not_found");

            if (phoneNumber != null)
            {
                var phoneResult = user.UpdatePhoneNumber(phoneNumber);
                if (phoneResult.IsFailure)
                    return Error.BadRequest(phoneResult.Error.Code);
            }

            if (gender.HasValue)
            {
                var genderResult = user.UpdateGender(gender.Value);
                if (genderResult.IsFailure)
                    return Error.BadRequest(genderResult.Error.Code);
            }

            if (preferredTwoFactorProvider.HasValue)
            {
                var providerResult = user.UpdatePreferredTwoFactorProvider(preferredTwoFactorProvider.Value);
                if (providerResult.IsFailure)
                    return Error.BadRequest(providerResult.Error.Code);
            }

            return Result.Success();
        }
    }
}
