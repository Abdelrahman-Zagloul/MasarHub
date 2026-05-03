using MasarHub.API.Middlewares;

namespace MasarHub.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseApiPipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseMiddleware<CultureMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}
