
namespace MasarHub.Application.Common.Results.Errors
{
    public sealed record Error(string Code, ErrorType Type, Dictionary<string, object?>? Metadata = null)
    {
        public static Error BadRequest(string code, string? propertyName = null, Dictionary<string, object?>? metadata = null)

            => new(code, ErrorType.BadRequest, BuildMetadata(propertyName, metadata));

        public static Error Validation(string code, string propertyName, Dictionary<string, object?>? metadata = null)
            => new(code, ErrorType.Validation, BuildMetadata(propertyName, metadata));

        public static Error Unauthorized(string code)
            => new(code, ErrorType.Unauthorized);

        public static Error Forbidden(string code)
            => new(code, ErrorType.Forbidden);

        public static Error NotFound(string code)
            => new(code, ErrorType.NotFound);
        public static Error Conflict(string code, string? propertyName = null)
            => new(code, ErrorType.Conflict, BuildMetadata(propertyName, null));

        public static Error Failure(string code)
            => new(code, ErrorType.Failure);

        private static Dictionary<string, object?> BuildMetadata(string? propertyName, Dictionary<string, object?>? metadata)
        {
            metadata ??= [];
            if (!string.IsNullOrWhiteSpace(propertyName))
                metadata["PropertyName"] = propertyName;

            return metadata;
        }
    }
}