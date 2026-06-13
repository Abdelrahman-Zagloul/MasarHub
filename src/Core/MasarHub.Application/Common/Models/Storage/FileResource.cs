namespace MasarHub.Application.Common.Models.Storage
{
    public sealed record FileResource
    (
        string FileName,
        string ContentType,
        Stream Content,
        long FileSizeInByte
    );
}
