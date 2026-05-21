using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MasarHub.API.Filters
{
    public sealed class RemoveApiVersionParametersFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                return;

            operation.Parameters = operation.Parameters
                .Where(p => !p.Name!.Equals("api-version", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
