namespace MasarHub.Application.Common.Models
{
    public sealed record FileResource
    (
        FileType FileType,
        string FileName,
        string ContentType,
        Stream Content,
        long Length
    );
    public enum FileType
    {
        Image,
        Video,
        Document
    }
}
