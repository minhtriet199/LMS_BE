
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LMS_BE.Swagger;

public class SwaggerCsrfOperationFilter : IOperationFilter
{
    public void Apply(
        OpenApiOperation operation,
        OperationFilterContext context)
    {
        var path = context.ApiDescription.RelativePath?
            .TrimEnd('/')
            .ToLowerInvariant();

        if (path is not ("api/auth/login"
                     or "api/auth/refresh"
                     or "api/auth/logout"))
        {
            return;
        }

        operation.Parameters ??= new List<IOpenApiParameter>();
        operation.Parameters ??= new List<IOpenApiParameter>();

        // Kiểm tra header đã tồn tại chưa
        if (operation.Parameters.Any(p =>
            p.In == ParameterLocation.Header &&
            string.Equals(
                p.Name,
                "X-Requested-With",
                StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Requested-With",
            In = ParameterLocation.Header,
            Required = true,
            Description = "Required header for authentication requests.",
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Default = System.Text.Json.Nodes.JsonValue.Create("XMLHttpRequest")
            }
        });
    }
}
