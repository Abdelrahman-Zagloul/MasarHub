namespace MasarHub.Application.Common.Models
{
    public static class StorageFolders
    {
        public static class Courses
        {
            private const string Base = "courses";

            public const string Thumbnails = $"{Base}/thumbnails";
            public const string Videos = $"{Base}/videos";
            public const string Attachments = $"{Base}/attachments";
        }

        public static class Users
        {
            private const string Base = "users";

            public const string Avatars = $"{Base}/avatars";
        }
    }
}
