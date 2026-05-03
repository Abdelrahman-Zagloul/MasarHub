using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;

namespace MasarHub.Domain.Modules.Courses
{
    public sealed class CourseEnrollment : SoftDeletableEntity
    {
        public Guid UserId { get; private set; }
        public Guid CourseId { get; private set; }
        public Guid OrderId { get; private set; }
        public EnrollmentStatus Status { get; private set; }
        public DateTimeOffset EnrolledAt { get; private set; }
        public DateTimeOffset? CompletedAt { get; private set; }
        public decimal PaidAmount { get; private set; }

        private CourseEnrollment() { }

        private CourseEnrollment(Guid userId, Guid courseId, Guid orderId, decimal paidAmount)
        {
            UserId = userId;
            CourseId = courseId;
            OrderId = orderId;
            PaidAmount = paidAmount;
            Status = EnrollmentStatus.Active;
            EnrolledAt = DateTimeOffset.UtcNow;
        }

        public static Result<CourseEnrollment> Create(Guid userId, Guid courseId, Guid orderId, decimal paidAmount)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(userId, nameof(userId)),
                Guard.AgainstEmptyGuid(courseId, nameof(courseId)),
                Guard.AgainstEmptyGuid(orderId, nameof(orderId)),
                Guard.AgainstNegative(paidAmount, nameof(paidAmount))
            );

            if (error is not null)
                return error;

            return new CourseEnrollment(userId, courseId, orderId, paidAmount);
        }

        public Result MarkCompleted()
        {
            if (Status != EnrollmentStatus.Active)
                return CourseEnrollmentErrors.InvalidStatusTransition;

            Status = EnrollmentStatus.Completed;
            CompletedAt = DateTimeOffset.UtcNow;
            MarkAsUpdated();

            return Result.Success();
        }

        public Result Cancel()
        {
            if (Status != EnrollmentStatus.Active)
                return CourseEnrollmentErrors.InvalidStatusTransition;

            Status = EnrollmentStatus.Cancelled;
            MarkAsUpdated();

            return Result.Success();
        }

        public Result Delete() => MarkAsDeleted();
    }
}
