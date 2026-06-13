namespace MasarHub.Application.Common.Models.Storage
{
    public sealed record StoredFile
    (
        string FileKey,
        string FileName,
        string ContentType,
        long FileSizeInByte,
        string Url,
        double DurationInSecond = 0
    );
}
