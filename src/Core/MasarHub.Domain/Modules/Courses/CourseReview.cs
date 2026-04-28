using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;
using MasarHub.Domain.SharedKernel.Exceptions;

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
            UserId = Guard.AgainstEmptyGuid(userId, nameof(userId));
            CourseId = Guard.AgainstEmptyGuid(courseId, nameof(courseId));
            ReviewContent = reviewContent;
            SetRating(rating);
        }

        public static CourseReview Create(
            Guid userId,
            Guid courseId,
            double rating,
            string? reviewContent = null)
        {
            return new CourseReview(userId, courseId, rating, reviewContent);
        }

        public void UpdateRating(double rating)
        {
            SetRating(rating);
            MarkAsEdited();
        }

        public void UpdateContent(string? content)
        {
            ReviewContent = content;
            MarkAsEdited();
        }

        private void SetRating(double rating)
        {
            if (rating < 1 || rating > 5)
                throw new DomainException(ErrorCodes.CourseReview.InvalidRating);

            Rating = rating;
        }

        private void MarkAsEdited()
        {
            EditedAt = DateTimeOffset.UtcNow;
            MarkAsUpdated();
        }

        public void Delete()
        {
            MarkAsDeleted();
        }
    }
}