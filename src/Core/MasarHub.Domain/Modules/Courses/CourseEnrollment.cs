using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;
using MasarHub.Domain.SharedKernel.Exceptions;

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
            UserId = Guard.AgainstEmptyGuid(userId, nameof(userId));
            CourseId = Guard.AgainstEmptyGuid(courseId, nameof(courseId));
            OrderId = Guard.AgainstEmptyGuid(orderId, nameof(orderId));
            PaidAmount = Guard.AgainstNegative(paidAmount, nameof(paidAmount));

            Status = EnrollmentStatus.Active;
            EnrolledAt = DateTimeOffset.UtcNow;
        }


        public static CourseEnrollment Create(Guid userId, Guid courseId, Guid orderId, decimal paidAmount)
        {
            return new CourseEnrollment(userId, courseId, orderId, paidAmount);
        }

        public void MarkCompleted()
        {
            if (Status != EnrollmentStatus.Active)
                throw new DomainException(ErrorCodes.CourseEnrollment.InvalidStatusTransition);

            Status = EnrollmentStatus.Completed;
            CompletedAt = DateTimeOffset.UtcNow;
            MarkAsUpdated();
        }
        public void Cancel()
        {
            if (Status != EnrollmentStatus.Active)
                throw new DomainException(ErrorCodes.CourseEnrollment.InvalidStatusTransition);

            Status = EnrollmentStatus.Cancelled;
            MarkAsUpdated();
        }
        public void Delete() => MarkAsDeleted();
    }
}