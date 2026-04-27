namespace MasarHub.Domain.SharedKernel
{
    public static class ErrorCodes
    {
        public static class General
        {
            public const string Null = "general.null";
            public const string Empty = "general.empty";
            public const string NullOrEmpty = "general.null_or_empty";
            public const string Invalid = "general.invalid";
            public const string Negative = "general.negative";
            public const string NegativeOrZero = "general.negative_or_zero";
            public const string EmptyGuid = "general.empty_guid";
            public const string InvalidEnum = "general.invalid_enum";
        }

        public static class Category
        {
            public const string MaxDepth = "category.max_depth";
            public const string InvalidParent = "category.invalid_parent";
        }
        public static class Course
        {
            public const string NotPendingApproval = "course.not_pending_approval";
            public const string InvalidStatusTransition = "course.invalid_status_transition";
            public const string AlreadyPublished = "course.already_published";
            public const string MissingThumbnail = "course.missing_thumbnail";
            public const string MissingPrerequisites = "course.missing_prerequisites";
        }
    }
}