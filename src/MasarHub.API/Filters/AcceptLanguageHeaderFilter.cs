using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Nodes;

namespace MasarHub.API.Filters
{
    public sealed class AcceptLanguageHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= [];

            if (operation.Parameters.Any(parameter => string.Equals(parameter.Name, "Accept-Language", StringComparison.OrdinalIgnoreCase)))
                return;

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Required = false,
                Description = "Language of the response.",
                Schema = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Default = JsonValue.Create("en"),
                    Enum =
                    [
                        JsonValue.Create("en"),
                        JsonValue.Create("ar")
                    ],

                },
            });
        }
    }
}
