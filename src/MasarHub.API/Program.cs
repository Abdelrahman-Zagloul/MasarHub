using MasarHub.API.Extensions;
using MasarHub.Application.Extensions;
using MasarHub.Infrastructure.Extensions;

namespace MasarHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services
                .AddAPI(builder.Configuration)
                .AddApplication()
                .AddInfrastructure(builder.Configuration);


            var app = builder.Build();

            app.UseApiPipeline();
            app.Run();
        }
    }
}
