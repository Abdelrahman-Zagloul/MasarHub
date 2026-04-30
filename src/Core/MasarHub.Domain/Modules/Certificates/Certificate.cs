using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;

namespace MasarHub.Domain.Modules.Certificates
{
    public sealed class Certificate : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid CourseId { get; private set; }
        public Guid EnrollmentId { get; private set; }
        public Guid TemplateId { get; private set; }
        public string CertificateNumber { get; private set; } = null!;
        public string VerificationCode { get; private set; } = null!;
        public DateTimeOffset IssuedAt { get; private set; }

        private Certificate() { }

        private Certificate(
            Guid userId,
            Guid courseId,
            Guid enrollmentId,
            Guid templateId,
            string certificateNumber,
            string verificationCode)
        {
            UserId = Guard.AgainstEmptyGuid(userId, nameof(userId));
            CourseId = Guard.AgainstEmptyGuid(courseId, nameof(courseId));
            EnrollmentId = Guard.AgainstEmptyGuid(enrollmentId, nameof(enrollmentId));
            TemplateId = Guard.AgainstEmptyGuid(templateId, nameof(templateId));

            CertificateNumber = Guard.AgainstNullOrWhiteSpace(certificateNumber, nameof(certificateNumber));
            VerificationCode = Guard.AgainstNullOrWhiteSpace(verificationCode, nameof(verificationCode));

            IssuedAt = DateTimeOffset.UtcNow;
        }

        public static Certificate Issue(Guid userId, Guid courseId, Guid enrollmentId, Guid templateId, string certificateNumber, string verificationCode)
        {
            return new Certificate(userId, courseId, enrollmentId, templateId, certificateNumber, verificationCode);
        }
    }
}