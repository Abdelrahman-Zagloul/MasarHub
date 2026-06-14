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
            var action = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            var statusCode = GetStatusCode(context.ApiDescription.HttpMethod);

            if (successType == null || successType == typeof(Unit) || successType == typeof(Task))
            {
                operation.Responses?.TryAdd(statusCode, new OpenApiResponse { Description = "Success" });
                return;
            }

            var schema = context.SchemaGenerator.GenerateSchema(successType, context.SchemaRepository);
            operation.Responses?.TryAdd(statusCode, new OpenApiResponse
            {
                Description = "Success",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new() { Schema = schema }
                }
            });
        }
        private static void AddErrorResponses(OpenApiOperation operation, OperationFilterContext context, ControllerActionDescriptor action)
        {
            var requiresAuth = !IsAnonymous(action) && HasAuthorizeAttribute(action);
            var hasRolesOrPolicy = requiresAuth && HasAttributeWithRolesOrPolicy(action);

            var validationErrorSchema = GetOrCreateSchema(context, "MasarHubApiValidationError", includeErrors: true);
            var errorSchema = GetOrCreateSchema(context, "MasarHubApiError", includeErrors: false);

            var schemaMap = new List<(string Code, string Description, IOpenApiSchema Schema)>
            {
                ("400", "Bad Request - Validation or Malformed Data", validationErrorSchema)
            };

            if (requiresAuth)
                schemaMap.Add(("401", "Unauthorized - Missing or Invalid Token", errorSchema));

            if (hasRolesOrPolicy)
                schemaMap.Add(("403", "Forbidden - Insufficient Permissions", errorSchema));

            schemaMap.Add(("404", "Not Found - Resource doesn't exist", errorSchema));
            schemaMap.Add(("409", "Conflict - Business Rule Violation", errorSchema));
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
        private static string GetStatusCode(string? httpMethod)
        {
            return httpMethod?.ToUpperInvariant() switch
            {
                "POST" => "201",
                "PUT" or "PATCH" or "DELETE" => "204",
                _ => "200"
            };
        }

        private static bool IsAnonymous(ControllerActionDescriptor action)
        {
            return action.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        }
        private static bool HasAuthorizeAttribute(ControllerActionDescriptor action)
        {
            return action.MethodInfo.GetCustomAttributes<AuthorizeAttribute>(true).Any()
                || action.ControllerTypeInfo.GetCustomAttributes<AuthorizeAttribute>(true).Any();
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
        private static IOpenApiSchema GetOrCreateSchema(OperationFilterContext context, string schemaId, bool includeErrors)
        {
            if (context.SchemaRepository.Schemas.TryGetValue(schemaId, out var existing))
                return existing;

            var properties = new Dictionary<string, IOpenApiSchema>
            {
                ["type"] = new OpenApiSchema { Type = JsonSchemaType.String, Description = "URI reference identifying the problem type." },
                ["title"] = new OpenApiSchema { Type = JsonSchemaType.String, Description = "Short, human-readable summary of the problem." },
                ["status"] = new OpenApiSchema { Type = JsonSchemaType.Integer, Description = "HTTP status code." },
                ["detail"] = new OpenApiSchema { Type = JsonSchemaType.String, Description = "Human-readable explanation of the problem." },
                ["instance"] = new OpenApiSchema { Type = JsonSchemaType.String, Description = "URI reference identifying the specific occurrence." },
                ["traceId"] = new OpenApiSchema { Type = JsonSchemaType.String, Description = "Trace identifier for tracking errors." }
            };

            if (includeErrors)
            {
                properties["errors"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    Description = "Validation errors keyed by property name.",
                    AdditionalProperties = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchema { Type = JsonSchemaType.String }
                    }
                };
            }

            var schema = new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = properties
            };

            context.SchemaRepository.Schemas[schemaId] = schema;
            return schema;
        }
    }
}