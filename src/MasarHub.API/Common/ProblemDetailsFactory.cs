using MasarHub.Application.Abstractions.Localization;
using MasarHub.Application.Common.Results;
using MasarHub.Application.Common.Results.Errors;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Common
{

    public static class ProblemDetailsFactory
    {
        private sealed record ValidationProblemErrors(Dictionary<string, List<string>> Errors);

        public static async Task<IActionResult> CreateAsync(Result result, ControllerBase controller, ILocalizationService localizer)
        {
            if (result.Errors.All(e => e.Type == ErrorType.Validation))
                return await HandleValidationAsync(result, controller, localizer);

            return await HandleFailureAsync(result, controller, localizer);
        }

        private static async Task<IActionResult> HandleFailureAsync(Result result, ControllerBase controller, ILocalizationService localizer)
        {
            var error = result.Errors.First();
            int statusCode = ToStatusCode(error.Type);

            var problem = new ProblemDetails
            {
                Title = error.Code,
                Detail = await localizer.GetAsync(error.Code),
                Status = statusCode,
                Type = error.Code,
                Instance = controller.Request.Path,
            };

            return controller.StatusCode(statusCode, problem);
        }

        private static async Task<IActionResult> HandleValidationAsync(Result result, ControllerBase controller, ILocalizationService localizer)
        {
            var errors = new Dictionary<string, List<string>>();
            foreach (var e in result.Errors)
            {
                var message = await localizer.GetAsync(e.Code);

                if (!errors.ContainsKey(e.PropertyName ?? "General"))
                    errors[e.PropertyName ?? "General"] = new List<string>();

                errors[e.PropertyName ?? "General"].Add(message);
            }

            var problem = new ProblemDetails
            {
                Title = "Validation Error",
                Detail = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Type = "validation",
                Instance = controller.Request.Path
            };

            problem.Extensions["errors"] = new ValidationProblemErrors(errors);

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
    }
}
