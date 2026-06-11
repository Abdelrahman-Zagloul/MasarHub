using System.ComponentModel.DataAnnotations;

namespace MasarHub.Application.Settings
{
    public sealed class FileStorageSettings
    {
        [Required]
        public int MaxImageSizeInMB { get; set; }

        [Required]
        public int MaxVideoSizeInMB { get; set; }

        [Required]
        public int MaxDocumentSizeInMB { get; set; }

        [Required]
        public int MaxAttachmentSizeInMB { get; set; }

        [Required]
        public string[] AllowedImageExtensions { get; set; } = [];

        [Required]
        public string[] AllowedVideoExtensions { get; set; } = [];

        [Required]
        public string[] AllowedDocumentExtensions { get; set; } = [];

        [Required]
        public string[] AllowedAttachmentExtensions { get; set; } = [];
    }
}
