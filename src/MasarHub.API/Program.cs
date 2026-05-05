using MasarHub.API.Extensions;
using MasarHub.Application.Extensions;
using MasarHub.Infrastructure.Extensions;
using Serilog;

namespace MasarHub.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.AddSerilog();
                builder.Services.AddControllers();
                builder.Services.AddOpenApi();

                builder.Services
                    .AddAPI(builder.Configuration)
                    .AddApplication()
                    .AddInfrastructure(builder.Configuration);


                var app = builder.Build();

                app.UseSerilogRequestLogging();

                await app.UseApiPipeline();
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
