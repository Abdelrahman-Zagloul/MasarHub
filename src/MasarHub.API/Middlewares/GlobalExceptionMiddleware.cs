using MasarHub.Application.Abstractions.Services.Localization;
using MasarHub.Application.Common.Results.Errors;
using Microsoft.AspNetCore.Mvc;

namespace MasarHub.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment webHostEnvironment)
        {
            _next = next;
            _logger = logger;
            _environment = webHostEnvironment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var _localizer = context.RequestServices.GetRequiredService<ILocalizationService>();

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var problemDetails = new ProblemDetails
            {
                Type = "https://httpstatuses.com/500",
                Title = await _localizer.GetAsync(ErrorType.Failure.ToString()),
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path
            };

            problemDetails.Extensions["traceId"] = context.TraceIdentifier;

            if (_environment.IsDevelopment())
                problemDetails.Extensions["details"] = exception.Message;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}