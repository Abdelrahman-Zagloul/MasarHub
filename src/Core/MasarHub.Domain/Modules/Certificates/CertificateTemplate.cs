using MasarHub.Domain.SharedKernel;
using MasarHub.Domain.SharedKernel.Base;

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
            Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name));
            HtmlContent = Guard.AgainstNullOrWhiteSpace(htmlContent, nameof(htmlContent));
            PreviewImageUrl = Guard.AgainstNullOrWhiteSpace(previewImageUrl, nameof(previewImageUrl));
        }

        public static CertificateTemplate Create(string name, string htmlContent, string previewImageUrl)
        {
            return new CertificateTemplate(name, htmlContent, previewImageUrl);
        }

        public void UpdateContent(string htmlContent)
        {
            HtmlContent = Guard.AgainstNullOrWhiteSpace(htmlContent, nameof(htmlContent));
            MarkAsUpdated();
        }

        public void UpdatePreview(string previewImageUrl)
        {
            PreviewImageUrl = Guard.AgainstNullOrWhiteSpace(previewImageUrl, nameof(previewImageUrl));
            MarkAsUpdated();
        }
    }
}