namespace MasarHub.Application.Settings
{
    public class DefaultUsersSettings
    {
        public bool SeedDefaultUsers { get; set; }
        public string AdminEmail { get; set; } = null!;
        public string AdminPassword { get; set; } = null!;
        public string InstructorEmail { get; set; } = null!;
        public string InstructorPassword { get; set; } = null!;
        public string StudentEmail { get; set; } = null!;
        public string StudentPassword { get; set; } = null!;
    }
}
