using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;
using MasarHub.Domain.SharedKernel.Exceptions;

namespace MasarHub.Domain.Modules.Courses
{
    public sealed class CourseEnrollment : SoftDeletableEntity
    {
        public Guid UserId { get; private set; }
        public Guid CourseId { get; private set; }
        public DateTimeOffset EnrolledAt { get; private set; }
        public decimal PaidAmount { get; private set; }
        public Guid? PaymentId { get; private set; }

        private CourseEnrollment() { }

        private CourseEnrollment(Guid userId, Guid courseId, decimal paidAmount, Guid? paymentId)
        {
            UserId = Guard.AgainstEmptyGuid(userId, nameof(userId));
            CourseId = Guard.AgainstEmptyGuid(courseId, nameof(courseId));
            PaidAmount = Guard.AgainstNegative(paidAmount, nameof(paidAmount));

            ValidatePayment(paidAmount, paymentId);

            PaymentId = paymentId;
            EnrolledAt = DateTimeOffset.UtcNow;
        }

        public static CourseEnrollment Create(
            Guid userId,
            Guid courseId,
            decimal paidAmount,
            Guid? paymentId = null)
        {
            return new CourseEnrollment(userId, courseId, paidAmount, paymentId);
        }

        public void Delete() => MarkAsDeleted();


        private void ValidatePayment(decimal paidAmount, Guid? paymentId)
        {
            var hasPayment = paymentId is not null && paymentId != Guid.Empty;

            if (paidAmount > 0 && !hasPayment)
                throw new DomainException(ErrorCodes.CourseEnrollment.MissingPayment);

            if (paidAmount == 0 && hasPayment)
                throw new DomainException(ErrorCodes.CourseEnrollment.InvalidPayment);
        }
    }
}