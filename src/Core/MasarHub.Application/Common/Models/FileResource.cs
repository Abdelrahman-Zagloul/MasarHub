namespace MasarHub.Application.Common.Models
{
    public sealed record FileResource
    (
        string FileName,
        string ContentType,
        Stream Content,
        long Length
    );
}
