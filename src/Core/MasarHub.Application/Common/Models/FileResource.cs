namespace MasarHub.Application.Common.Models
{
    public sealed record FileResource
    (
        string FileName,
        string ContentType,
        Stream Content,
        long FileSizeInByte
    );
    public enum FileType
    {
        Image,
        Video,
        Document
    }
}
