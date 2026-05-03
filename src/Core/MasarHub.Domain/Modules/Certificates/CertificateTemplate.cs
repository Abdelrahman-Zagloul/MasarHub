using MasarHub.Domain.Common.Base;
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

        public static Result<CertificateTemplate> Create(string name, string htmlContent, string previewImageUrl)
        {
            var error = GuardExtensions.FirstError(
                Guard.AgainstNullOrWhiteSpace(name, nameof(name)),
                Guard.AgainstNullOrWhiteSpace(htmlContent, nameof(htmlContent)),
                Guard.AgainstNullOrWhiteSpace(previewImageUrl, nameof(previewImageUrl))
            );

            if (error is not null)
                return Result<CertificateTemplate>.Failure(error);

            return new CertificateTemplate(name, htmlContent, previewImageUrl);
        }

        public Result UpdateContent(string htmlContent)
        {
            var error = Guard.AgainstNullOrWhiteSpace(htmlContent, nameof(htmlContent));
            if (error is not null)
                return error;

            HtmlContent = htmlContent;
            MarkAsUpdated();

            return Result.Success();
        }

        public Result UpdatePreview(string previewImageUrl)
        {
            var error = Guard.AgainstNullOrWhiteSpace(previewImageUrl, nameof(previewImageUrl));
            if (error is not null)
                return error;

            PreviewImageUrl = previewImageUrl;
            MarkAsUpdated();

            return Result.Success();
        }
    }
}