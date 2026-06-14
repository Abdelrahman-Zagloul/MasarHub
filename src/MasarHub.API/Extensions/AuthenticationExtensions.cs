using MasarHub.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MasarHub.API.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JWTSettings)).Get<JWTSettings>()
            ?? throw new InvalidOperationException("JWTSettings not configured");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/problem+json";
                        await context.Response.WriteAsJsonAsync(new ProblemDetails
                        {
                            Type = "https://httpstatuses.com/401",
                            Title = "Unauthorized",
                            Detail = "Authentication is required.",
                            Status = StatusCodes.Status401Unauthorized,
                            Instance = context.Request.Path,
                            Extensions =
                            {
                                ["traceId"] = context.HttpContext.TraceIdentifier
                            }
                        });
                    },

                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/problem+json";

                        await context.Response.WriteAsJsonAsync(new ProblemDetails
                        {
                            Type = "https://httpstatuses.com/403",
                            Title = "Forbidden",
                            Detail = "You do not have permission to access this resource.",
                            Status = StatusCodes.Status403Forbidden,
                            Instance = context.Request.Path,
                            Extensions =
                            {
                                ["traceId"] = context.HttpContext.TraceIdentifier
                            }
                        });
                    }
                };
            });

        return services;
    }
}