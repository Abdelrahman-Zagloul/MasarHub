using MasarHub.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace MasarHub.API.Filters
{
    public sealed class AutoResponseTypeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor is not ControllerActionDescriptor controllerAction)
                return;

            var successType = InferSuccessType(controllerAction);

            var hasSuccessResponse = controllerAction.MethodInfo
                .GetCustomAttributes<ProducesResponseTypeAttribute>()
                .Any(x => x.StatusCode is >= 200 and < 300);

            if (!hasSuccessResponse)
            {
                RemoveExistingSuccessResponses(operation);
                AddSuccessResponse(operation, context, successType);
            }
            AddErrorResponses(operation, context, controllerAction);
        }

        private static Type? InferSuccessType(ControllerActionDescriptor action)
        {
            var mediatRParam = action.MethodInfo.GetParameters()
                .FirstOrDefault(p => p.ParameterType.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)));

            if (mediatRParam is null)
                return null;

            var requestInterface = mediatRParam.ParameterType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

            var responseType = requestInterface.GetGenericArguments()[0];
            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
                return responseType.GetGenericArguments()[0];

            return null;
        }
        private static void RemoveExistingSuccessResponses(OpenApiOperation operation)
        {
            var successCodes = operation.Responses?.Keys
                .Where(k => int.TryParse(k, out var code) && code is >= 200 and < 300)
                .ToList();

            if (successCodes == null) return;

            foreach (var code in successCodes)
                operation.Responses?.Remove(code);
        }
        private static void AddSuccessResponse(OpenApiOperation operation, OperationFilterContext context, Type? successType)
        {
            var statusCode = GetStatusCode(context.ApiDescription.HttpMethod);

            if (successType == null || successType == typeof(Unit) || successType == typeof(Task))
            {
                operation.Responses?.TryAdd(statusCode, new OpenApiResponse { Description = "Success" });
                return;
            }

            var schema = context.SchemaGenerator.GenerateSchema(successType, context.SchemaRepository);

            if (schema is OpenApiSchema openApiSchema)
            {
                operation.Responses?.TryAdd(statusCode, new OpenApiResponse
                {
                    Description = "Success",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new() { Schema = openApiSchema }
                    }
                });
            }
        }
        private static void AddErrorResponses(OpenApiOperation operation, OperationFilterContext context, ControllerActionDescriptor action)
        {
            var requiresAuth = RequiresAuthorization(action);
            var hasRolesOrPolicy = requiresAuth && HasAttributeWithRolesOrPolicy(action);

            var validationErrorSchema = GetAndEnrichSchema(context, typeof(ValidationProblemDetails).Name, "traceId", JsonSchemaType.String, "Trace identifier for tracking and debugging errors.");
            var errorSchema = GetAndEnrichSchema(context, typeof(ProblemDetails).Name, "traceId", JsonSchemaType.String, "Trace identifier for tracking and debugging errors.");

            var schemaMap = new List<(string Code, string Description, OpenApiSchema Schema)>
            {
                ("400", "Bad Request - Validation or Malformed Data", validationErrorSchema)
            };

            if (requiresAuth)
                schemaMap.Add(("401", "Unauthorized - Missing or Invalid Token", errorSchema));

            if (hasRolesOrPolicy)
                schemaMap.Add(("403", "Forbidden - Insufficient Permissions", errorSchema));

            schemaMap.Add(("404", "Not Found - Resource Not Found", errorSchema));
            schemaMap.Add(("409", "Conflict - Conflict or Business Rule Violation", errorSchema));
            schemaMap.Add(("500", "Internal Server Error", errorSchema));

            foreach (var (code, description, schema) in schemaMap)
            {
                operation.Responses?.TryAdd(code, new OpenApiResponse
                {
                    Description = description,
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new() { Schema = schema }
                    }
                });
            }
        }
        private static OpenApiSchema GetAndEnrichSchema(OperationFilterContext context, string schemaKey, string propertyName, JsonSchemaType type, string description)
        {
            var baseSchema = context.SchemaGenerator.GenerateSchema(schemaKey == typeof(ProblemDetails).Name
                ? typeof(ProblemDetails)
                : typeof(ValidationProblemDetails), context.SchemaRepository);

            if (context.SchemaRepository.Schemas.TryGetValue(schemaKey, out var schema) && schema is OpenApiSchema concreteSchema)
            {
                if (concreteSchema.Properties == null)
                {
                    concreteSchema.Properties = new Dictionary<string, IOpenApiSchema>();
                }

                if (!concreteSchema.Properties.ContainsKey(propertyName))
                {
                    concreteSchema.Properties.Add(propertyName, new OpenApiSchema
                    {
                        Type = type,
                        Description = description
                    });
                }

                concreteSchema.AdditionalPropertiesAllowed = false;
                concreteSchema.AdditionalProperties = null;

                return concreteSchema;
            }

            return baseSchema as OpenApiSchema ?? new OpenApiSchema();
        }
        private static string GetStatusCode(string? httpMethod)
        {
            return httpMethod?.ToUpperInvariant() switch
            {
                "POST" => "201",
                "PUT" or "PATCH" or "DELETE" => "204",
                _ => "200"
            };
        }
        private static bool RequiresAuthorization(ControllerActionDescriptor action)
        {
            var isAnonymous = action.EndpointMetadata
                .OfType<AllowAnonymousAttribute>()
                .Any();

            var hasAuthorize = action.MethodInfo
                .GetCustomAttributes<AuthorizeAttribute>(true).Any()
                || action.ControllerTypeInfo.GetCustomAttributes<AuthorizeAttribute>(true).Any();
            return !isAnonymous && hasAuthorize;
        }
        private static bool HasAttributeWithRolesOrPolicy(ControllerActionDescriptor action)
        {
            var attributes = action.MethodInfo
                .GetCustomAttributes<AuthorizeAttribute>(true)
                .Concat(action.ControllerTypeInfo.GetCustomAttributes<AuthorizeAttribute>(true));

            return attributes.Any(a =>
                !string.IsNullOrWhiteSpace(a.Roles) ||
                !string.IsNullOrWhiteSpace(a.Policy));
        }
    }
}