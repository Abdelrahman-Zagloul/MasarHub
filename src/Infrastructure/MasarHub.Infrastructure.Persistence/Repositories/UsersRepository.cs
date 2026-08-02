using MasarHub.Application.Abstractions.Persistence.Repositories;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
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

            user.UpdateProfileImage(newPublicId);
            return Result.Success();
        }
    }
}
