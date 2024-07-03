using System.ComponentModel;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPI.Swagger;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class RequestedWithXmlHttpRequest : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Requested-With",
            In = ParameterLocation.Header,
            Description = "How the request was made (keep the default)",
            Required = false,
            Example = new OpenApiString("XMLHttpRequest"),
            Schema = new OpenApiSchema
            {
                Title = "Key",
                Type = "string",
                Default = new OpenApiString("XMLHttpRequest"),
            },
        });
    }
}