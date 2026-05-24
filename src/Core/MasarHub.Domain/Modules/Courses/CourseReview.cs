using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Courses
{
    public sealed class CourseReview : SoftDeletableEntity
    {
        public double Rating { get; private set; }
        public string? ReviewContent { get; private set; }
        public DateTimeOffset? EditedAt { get; private set; }
        public Guid UserId { get; private set; }
        public Guid CourseId { get; private set; }

        private CourseReview() { }

        private CourseReview(Guid userId, Guid courseId, double rating, string? reviewContent)
        {
            UserId = userId;
            CourseId = courseId;
            Rating = rating;
            ReviewContent = reviewContent;
        }

        public static DomainResult<CourseReview> Create(
            Guid userId,
            Guid courseId,
            double rating,
            string? reviewContent = null)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(userId, nameof(userId)),
                Guard.AgainstEmptyGuid(courseId, nameof(courseId))
            );
            if (error is not null)
                return error;

            if (!IsValidRating(rating))
                return CourseErrors.InvalidRating;

            return new CourseReview(userId, courseId, rating, reviewContent);
        }

        public DomainResult UpdateRating(double rating)
        {
            if (!IsValidRating(rating))
                return CourseErrors.InvalidRating;

            Rating = rating;
            MarkAsEdited();
            return DomainResult.Success();
        }

        public DomainResult UpdateContent(string? content)
        {
            ReviewContent = content;
            MarkAsEdited();
            return DomainResult.Success();
        }

        public DomainResult Delete() => MarkAsDeleted();

        private static bool IsValidRating(double rating) => rating is >= 1 and <= 5;

        private void MarkAsEdited()
        {
            EditedAt = DateTimeOffset.UtcNow;
            MarkAsUpdated();
        }
    }
}
