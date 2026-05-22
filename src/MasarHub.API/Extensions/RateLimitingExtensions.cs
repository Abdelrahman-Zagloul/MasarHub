using MasarHub.Application.Abstractions.Localization;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace MasarHub.API.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimitingConfiguration(this IServiceCollection services, int globalRequestsPerMinute = 100)
    {
        services.AddRateLimiter(options =>
        {
            options.AddPolicy(RateLimitingPolicies.Global, context =>
            {
                return CreateSlidingPolicy(context, globalRequestsPerMinute, TimeSpan.FromMinutes(1), 6);
            });

            options.AddPolicy(RateLimitingPolicies.Sensitive, context =>
            {
                return CreateSlidingPolicy(context, 10, TimeSpan.FromMinutes(5), 5);
            });

            options.AddPolicy(RateLimitingPolicies.Otp, context =>
            {
                return CreateSlidingPolicy(context, 3, TimeSpan.FromMinutes(5), 5);
            });

            options.AddPolicy(RateLimitingPolicies.Strict, context =>
            {
                return CreateSlidingPolicy(context, 3, TimeSpan.FromMinutes(10), 5);
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, ct) => await BuildRejectionResponse(context, ct);
        });

        return services;
    }

    private static string GetIp(HttpContext context) => context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    private static RateLimitPartition<string> CreateSlidingPolicy(HttpContext context, int permitLimit, TimeSpan window, int segmentsPerWindow)
    {
        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: GetIp(context),
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = window,
                SegmentsPerWindow = segmentsPerWindow,
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    }
    private static async Task BuildRejectionResponse(OnRejectedContext context, CancellationToken ct)
    {
        var localizer = context.HttpContext.RequestServices.GetRequiredService<ILocalizationService>();

        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = await localizer.GetAsync("TooManyRequests"),
        }, ct);
    }
}
public static class RateLimitingPolicies
{
    public const string Global = "global";

    public const string Sensitive = "sensitive";

    public const string Otp = "otp";

    public const string Strict = "strict";
}
