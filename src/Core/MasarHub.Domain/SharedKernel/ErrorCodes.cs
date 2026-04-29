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
            public const string AlreadyDeleted = "general.already_deleted";
            public const string NotDeleted = "general.not_deleted";
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
            public const string DuplicateModuleOrder = "course.duplicate_module_order";
            public const string DuplicateLessonOrder = "course.duplicate_lesson_order";
        }
        public static class CourseReview
        {
            public const string InvalidRating = "course_review.invalid_rating";
        }
        public static class CourseEnrollment
        {
            public const string MissingPayment = "course_enrollment.missing_payment";
            public const string InvalidPayment = "course_enrollment.invalid_payment";
        }
        public static class Payment
        {
            public const string InvalidStatusTransition = "payment.invalid_status_transition";
        }
        public static class Coupon
        {
            public const string Expired = "coupon.expired";
            public const string Exhausted = "coupon.exhausted";
            public const string NotApplicableToCourse = "coupon.invalid_course";
            public const string InvalidPercentage = "coupon.invalid_percentage";
            public const string InvalidExpiration = "coupon.invalid_expiration";
        }
    }
}
