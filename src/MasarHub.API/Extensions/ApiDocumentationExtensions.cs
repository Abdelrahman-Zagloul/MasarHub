using MasarHub.API.Filters;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace MasarHub.API.Extensions
{
    public static class ApiDocumentationExtensions
    {
        public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<RemoveApiVersionParametersFilter>();
                options.OperationFilter<AcceptLanguageHeaderFilter>();
                options.OperationFilter<AutoResponseTypeOperationFilter>();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MasarHub API",
                    Version = "v1",
                    Description = "API documentation for MasarHub.",
                    Contact = new OpenApiContact
                    {
                        Name = "MasarHub System Support",
                        Email = "abdelrahman.zagloul.dev@gmail.com",
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    },
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token"
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document, null)] = []
                });
            });

            return services;
        }

        public static WebApplication UseSwaggerDocumentation(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DocumentTitle = "MasarHub API - Swagger";
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "MasarHub API v1");
                options.RoutePrefix = "swagger";
                options.EnableFilter();
                options.DocExpansion(DocExpansion.None);
                options.DisplayRequestDuration();
            });

            return app;
        }
        public static WebApplication UseScalarDocumentation(this WebApplication app)
        {
            app.MapScalarApiReference("/scalar", options =>
            {
                options
                    .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json")
                    .AddPreferredSecuritySchemes("Bearer")
                    .AddHttpAuthentication("Bearer", _ => { })
                    .EnablePersistentAuthentication();
            });
            return app;
        }

    }
}
