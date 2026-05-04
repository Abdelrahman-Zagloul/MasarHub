using System.ComponentModel.DataAnnotations;

namespace MasarHub.Application.Settings
{
    public sealed class DefaultUsersSettings
    {
        public bool SeedDefaultUsers { get; set; }

        [EmailAddress]
        public string AdminEmail { get; set; } = null!;

        [MinLength(6)]
        public string AdminPassword { get; set; } = null!;

        [EmailAddress]
        public string InstructorEmail { get; set; } = null!;

        [MinLength(6)]
        public string InstructorPassword { get; set; } = null!;

        [EmailAddress]
        public string StudentEmail { get; set; } = null!;

        [MinLength(6)]
        public string StudentPassword { get; set; } = null!;
    }
}
