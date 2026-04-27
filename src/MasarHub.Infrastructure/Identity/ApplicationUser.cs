
using Microsoft.AspNetCore.Identity;

namespace MasarHub.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = default!;
        public string? ProfileImageUrl { get; set; }

    }
}
