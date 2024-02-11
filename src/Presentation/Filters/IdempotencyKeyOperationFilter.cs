using Infrastructure.Idempotency;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Presentation.Filters;

public sealed class IdempotencyKeyOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        // see if the endpoint has RequireIdempotency Attribute
        var hasRequireIdempotency = context
            .ApiDescription
            .ActionDescriptor
            .EndpointMetadata
            .OfType<RequireIdempotencyAttribute>()
            .Any();

        if (hasRequireIdempotency)
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Idempotency-Key",
                In = ParameterLocation.Header,
                Description = "Guid Idempotency Key",
                Required = true,
                Examples = Enumerable.Range(0, 10)
                    .ToDictionary(k => k.ToString(), _ => new OpenApiExample
                    {
                        Value = new OpenApiString(Guid.NewGuid().ToString()),
                    }),
                Schema = new OpenApiSchema
                {
                    Title = "Key",
                    Type = "string",
                    Default = new OpenApiString(Guid.NewGuid().ToString()),
                    Pattern = "^([a-f0-9]{8}(-[a-f0-9]{4}){3}-[a-f0-9]{12})$",
                },
            });
    }
}