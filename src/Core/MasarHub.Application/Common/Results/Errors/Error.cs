
namespace MasarHub.Application.Common.Results.Errors
{
    public sealed record Error(string Code, ErrorType Type, string? PropertyName = null)
    {
        public static Error BadRequest(string code, string? propertyName = null)
            => new(code, ErrorType.BadRequest, propertyName);

        public static Error Validation(string code, string? propertyName = null)
            => new(code, ErrorType.Validation, propertyName);

        public static Error Unauthorized(string code)
            => new(code, ErrorType.Unauthorized);

        public static Error Forbidden(string code)
            => new(code, ErrorType.Forbidden);

        public static Error NotFound(string code)
            => new(code, ErrorType.NotFound);

        public static Error Conflict(string code)
            => new(code, ErrorType.Conflict);

        public static Error Failure(string code)
            => new(code, ErrorType.Failure);
    }
}