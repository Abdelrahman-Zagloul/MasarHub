using MasarHub.Infrastructure.Persistence.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MasarHub.Infrastructure.Persistence.Configurations.Identity
{
    public static class IdentityModelBuilderExtensions
    {
        public static void ApplyIdentitySchema(this ModelBuilder builder)
        {
            var schema = "identity";

            builder.Entity<ApplicationUser>().ToTable("Users", schema);
            builder.Entity<IdentityRole<Guid>>().ToTable("Roles", schema);
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", schema);
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", schema);
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", schema);
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", schema);
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", schema);
        }
    }
}
