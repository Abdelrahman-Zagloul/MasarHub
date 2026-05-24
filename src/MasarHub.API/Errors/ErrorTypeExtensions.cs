using MasarHub.Application.Common.Results.Errors;

namespace MasarHub.API.Errors
{
    public static class ErrorTypeExtensions
    {
        public static int ToStatusCode(this ErrorType type) => type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.BadRequest => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
