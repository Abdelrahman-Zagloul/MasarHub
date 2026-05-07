using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Common;

public static class ProblemDetailsFactory
{
    public static async Task<IActionResult> CreateAsync(Result result, ControllerBase controller, ILocalizationService localizer)
    {
        if (result.Errors.All(e => e.Type == ErrorType.Validation))
            return await HandleValidationAsync(result, controller, localizer);

        return await HandleFailureAsync(result, controller, localizer);
    }

    private static async Task<IActionResult> HandleFailureAsync(
        Result result,
        ControllerBase controller,
        ILocalizationService localizer)
    {
        var error = result.Errors.First();

        var detail = await localizer.GetAsync(error.Code, error.Metadata);
        var statusCode = ToStatusCode(error.Type);

        var problem = new ProblemDetails
        {
            Title = await localizer.GetAsync(error.Type.ToString()),
            Detail = detail,
            Status = statusCode,
            Instance = controller.Request.Path,
        };
        problem.Extensions["traceId"] = controller.HttpContext.TraceIdentifier;

        return controller.StatusCode(statusCode, problem);
    }

    private static async Task<IActionResult> HandleValidationAsync(
        Result result,
        ControllerBase controller,
        ILocalizationService localizer)
    {
        var errors = new Dictionary<string, List<string>>();
        foreach (var error in result.Errors)
        {
            var propertyName = GetPropertyName(error.Metadata);

            var message = await localizer.GetAsync(error.Code, error.Metadata);
            if (!errors.ContainsKey(propertyName))
                errors[propertyName] = [];

            errors[propertyName].Add(message);
        }


        var problem = new ProblemDetails
        {
            Title = await localizer.GetAsync(ErrorType.Validation.ToString()),
            Detail = await localizer.GetAsync("one_or_more_validation"),
            Status = StatusCodes.Status400BadRequest,
            Instance = controller.Request.Path,
        };
        problem.Extensions["traceId"] = controller.HttpContext.TraceIdentifier;
        problem.Extensions["errors"] = errors;

        return controller.BadRequest(problem);
    }

    private static int ToStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.BadRequest => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string GetPropertyName(Dictionary<string, object?>? metadata)
        => metadata?.GetValueOrDefault("PropertyName")?.ToString() ?? "general";
}
