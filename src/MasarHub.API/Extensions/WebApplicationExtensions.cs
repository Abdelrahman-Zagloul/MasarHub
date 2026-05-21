using Hangfire;
using MasarHub.API.Middlewares;
using MasarHub.Application.Abstractions.Persistence;
using MasarHub.Application.Settings;
using MasarHub.Infrastructure.Extensions;
using Scalar.AspNetCore;

namespace MasarHub.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task<WebApplication> UseApiPipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerDocumentation();
                app.MapScalarApiReference();
                app.UseCors(CorsSettings.DevelopmentPolicy);
            }
            else
            {
                app.UseCors(CorsSettings.ProductionPolicy);
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseMiddleware<CultureMiddleware>();


            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapSignalRHubs();

            await app.InitializeAsync();

            app.UseHangfireDashboard();
            app.UseHangfireJobs();

            return app;
        }

        private static async Task InitializeAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            await initializer.InitializeAsync();
        }
    }
}
