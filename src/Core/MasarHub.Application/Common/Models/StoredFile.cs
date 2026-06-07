namespace MasarHub.Application.Common.Models
{
    public sealed record StoredFile
    (
        string FileKey,
        string FileName,
        string ContentType,
        long Length,
        string Url
    );
}
