using MasarHub.Domain.Common.Base;
using MasarHub.Domain.Common.Errors;
using MasarHub.Domain.Common.Guards;
using MasarHub.Domain.Common.Results;
using MasarHub.Domain.Modules.Certificates.Events;

namespace MasarHub.Domain.Modules.Certificates
{
    public sealed class CertificateTemplate : BaseEntity
    {
        public string Name { get; private set; } = null!;
        public string HtmlContent { get; private set; } = null!;
        public string PreviewImageUrl { get; private set; } = null!;

        private CertificateTemplate() { }

        private CertificateTemplate(string name, string htmlContent, string previewImageUrl)
        {
            Name = name;
            HtmlContent = htmlContent;
            PreviewImageUrl = previewImageUrl;

            RaiseDomainEvent(new CertificateTemplateCreatedEvent(Id));
        }

        public static DomainResult<CertificateTemplate> Create(string name, string htmlContent, string previewImageUrl)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstNullOrWhiteSpace(name, nameof(name)),
                Guard.AgainstNullOrWhiteSpace(htmlContent, nameof(htmlContent)),
                Guard.AgainstNullOrWhiteSpace(previewImageUrl, nameof(previewImageUrl))
            );

            if (error is not null)
                return DomainResult<CertificateTemplate>.Failure(error);

            return new CertificateTemplate(name, htmlContent, previewImageUrl);
        }

        public DomainResult UpdateContent(string htmlContent)
        {
            var error = Guard.AgainstNullOrWhiteSpace(htmlContent, nameof(htmlContent));
            if (error != DomainError.None)
                return error;

            HtmlContent = htmlContent;
            MarkAsUpdated();

            return DomainResult.Success();
        }

        public DomainResult UpdatePreview(string previewImageUrl)
        {
            var error = Guard.AgainstNullOrWhiteSpace(previewImageUrl, nameof(previewImageUrl));
            if (error != DomainError.None)
                return error;

            PreviewImageUrl = previewImageUrl;
            MarkAsUpdated();

            return DomainResult.Success();
        }
    }
}