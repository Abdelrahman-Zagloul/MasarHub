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

            public const string InvalidUrl = "general.invalid_url";

            public const string TooManyItems = "general.too_many_items";
            public const string Duplicate = "general.duplicate";
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
            public const string InvalidStatusTransition = "course_enrollment.invalid_status_transition";
        }
        public static class Order
        {
            public const string InvalidDiscount = "order.invalid_discount";
            public const string InvalidStatusTransition = "order.invalid_status_transition";

            public const string DuplicateCourse = "order.duplicate_course";
            public const string EmptyOrder = "order.empty";

            public const string PaymentNotRequired = "order.payment_not_required";
        }
        public static class Payment
        {
            public const string InvalidStatusTransition = "payment.invalid_status_transition";
        }

        public static class Coupon
        {
            public const string Expired = "coupon.expired";
            public const string Exhausted = "coupon.exhausted";

            public const string NotApplicableToCourse = "coupon.not_applicable_to_course";

            public const string InvalidPercentage = "coupon.invalid_percentage";
            public const string InvalidExpiration = "coupon.invalid_expiration";
        }
        public static class Certificate
        {
            public const string FileUrlAlreadySet = "certificate.file_url_already_set";
        }
        public static class Exam
        {
            // Exam-level errors
            public const string InvalidPassingScore = "Exam.InvalidPassingScore";
            public const string InvalidTotalMarks = "Exam.InvalidTotalMarks";

            public const string CannotModifyPublishedExam = "Exam.CannotModifyPublishedExam";
            public const string CannotUnpublishAfterAttempts = "Exam.CannotUnpublishAfterAttempts";


            // Question-level errors
            public const string MissingQuestions = "Exam.MissingQuestions";
            public const string QuestionNotFound = "Exam.QuestionNotFound";
            public const string InvalidQuestionExamRelation = "Exam.InvalidQuestionExamRelation";
            public const string DuplicateQuestion = "Exam.DuplicateQuestion";
            public const string InvalidQuestionType = "Exam.InvalidQuestionType";



            // Option-level errors
            public const string OptionNotFound = "Exam.OptionNotFound";
            public const string MultipleCorrectOptionsNotAllowed = "Exam.MultipleCorrectOptionsNotAllowed";
            public const string TrueFalseMaxOptions = "Exam.TrueFalseMaxOptions";
            public const string TrueFalseMustHaveTwoOptions = "Exam.TrueFalseMustHaveTwoOptions";
            public const string TrueFalseMustHaveOneCorrect = "Exam.TrueFalseMustHaveOneCorrect";
            public const string SingleChoiceMustHaveOneCorrect = "Exam.SingleChoiceMustHaveOneCorrect";
            public const string MultipleChoiceMustHaveAtLeastTwoCorrect = "Exam.MultipleChoiceMustHaveAtLeastTwoCorrect";
            public const string QuestionMustHaveOptions = "Exam.QuestionMustHaveOptions";


            // Attempt-level errors
            public const string AttemptLimitExceeded = "Exam.AttemptLimitExceeded";
            public const string ExamTimeExpired = "Exam.ExamTimeExpired";
        }

        public static class CourseAnnouncement
        {
            public const string AlreadyPublished = "course_announcement.already_published";
            public const string CannotEditAfterPublish = "course_announcement.cannot_edit_after_publish";
            public const string InvalidScheduleTime = "course_announcement.invalid_schedule_time";
            public const string InvalidExpirationTime = "course_announcement.invalid_expiration_time";
        }
    }
}
