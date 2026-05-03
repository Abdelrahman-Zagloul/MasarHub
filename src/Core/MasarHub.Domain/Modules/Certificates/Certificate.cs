using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Certificates.Events;
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
            UserId = userId;
            CourseId = courseId;
            EnrollmentId = enrollmentId;
            TemplateId = templateId;
            CertificateNumber = certificateNumber;
            VerificationCode = verificationCode;
            IssuedAt = DateTimeOffset.UtcNow;

            RaiseDomainEvent(new CertificateIssuedEvent(Id, userId, courseId));
        }

        public static Result<Certificate> Issue(Guid userId, Guid courseId, Guid enrollmentId, Guid templateId, string certificateNumber, string verificationCode)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstEmptyGuid(userId, nameof(userId)),
                Guard.AgainstEmptyGuid(courseId, nameof(courseId)),
                Guard.AgainstEmptyGuid(enrollmentId, nameof(enrollmentId)),
                Guard.AgainstEmptyGuid(templateId, nameof(templateId)),
                Guard.AgainstNullOrWhiteSpace(certificateNumber, nameof(certificateNumber)),
                Guard.AgainstNullOrWhiteSpace(verificationCode, nameof(verificationCode))
            );

            if (error is not null)
                return error;

            return new Certificate(userId, courseId, enrollmentId, templateId, certificateNumber, verificationCode);
        }
    }
}