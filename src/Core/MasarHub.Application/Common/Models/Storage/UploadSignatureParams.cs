namespace MasarHub.Application.Common.Models.Storage
{
    public sealed record UploadSignatureParams
    (
        string CloudName,
        string ApiKey,
        string Signature,
        long Timestamp,
        string Folder,
        string ResourceType,
        string Type
    );
}
