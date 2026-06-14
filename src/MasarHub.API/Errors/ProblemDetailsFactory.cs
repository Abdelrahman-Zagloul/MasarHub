using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Errors;

public static class ProblemDetailsFactory
{
    public static async Task<IActionResult> CreateAsync(Result result, ControllerBase controller, ILocalizationService localizer)
    {
        return result.Errors.All(e => e.Type == ErrorType.Validation)
            ? await HandleValidationAsync(result.Errors, controller, localizer)
            : await HandleFailureAsync(result.Errors[0], controller, localizer);
    }

    private static async Task<IActionResult> HandleFailureAsync(Error error, ControllerBase controller, ILocalizationService localizer)
    {
        var statusCode = error.Type.ToStatusCode();

        var detailTask = localizer.GetAsync(error.Code, error.Metadata);
        var titleTask = localizer.GetAsync(error.Type.ToString());

        await Task.WhenAll(detailTask, titleTask);

        var problem = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = await titleTask,
            Detail = await detailTask,
            Status = statusCode,
            Instance = controller.Request.Path,
            Extensions =
            {
                ["traceId"] = controller.HttpContext.TraceIdentifier,
            }
        };


        return controller.StatusCode(statusCode, problem);
    }

    private static async Task<IActionResult> HandleValidationAsync(IReadOnlyCollection<Error> validationErrors, ControllerBase controller, ILocalizationService localizer)
    {
        var titleTask = localizer.GetAsync(ErrorType.Validation.ToString());
        var detailTask = localizer.GetAsync("one_or_more_validation");

        var localizedErrorsTask = Task.WhenAll(validationErrors.Select(async error => new
        {
            PropertyName = GetPropertyName(error.Metadata),
            Message = await localizer.GetAsync(error.Code, error.Metadata)
        }));

        await Task.WhenAll(titleTask, detailTask, localizedErrorsTask);

        var localizedErrors = await localizedErrorsTask;
        var errors = localizedErrors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Message).ToArray());

        var problem = new ValidationProblemDetails()
        {
            Type = "https://httpstatuses.com/400",
            Title = await titleTask,
            Detail = await detailTask,
            Status = StatusCodes.Status400BadRequest,
            Instance = controller.Request.Path,
            Errors = errors,
            Extensions =
            {
                ["traceId"] = controller.HttpContext.TraceIdentifier
            }
        };

        return controller.BadRequest(problem);
    }

    private static string GetPropertyName(Dictionary<string, object?>? metadata)
        => metadata?.GetValueOrDefault("PropertyName")?.ToString() ?? "general";
}
