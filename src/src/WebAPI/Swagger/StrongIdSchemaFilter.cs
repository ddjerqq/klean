using System.ComponentModel;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPI.Swagger;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class StrongIdSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var strongIdTypes = new[]
            {
                Assembly.Load(nameof(Domain)),
                Assembly.Load(nameof(Application)),
                Assembly.Load(nameof(Infrastructure)),
            }
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.Namespace?.Contains("Generated", StringComparison.InvariantCultureIgnoreCase) ?? false)
            .Where(type => type.IsValueType)
            .Where(type => type.Name.EndsWith("id", StringComparison.InvariantCultureIgnoreCase))
            .ToList();

        if (strongIdTypes.Contains(context.Type))
            schema.Type = "string";
    }
}