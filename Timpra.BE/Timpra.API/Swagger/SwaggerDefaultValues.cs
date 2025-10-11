 using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Timpra.API.Swagger;

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiVersion = context.ApiDescription.GroupName;
        if (operation.Parameters == null) return;

        foreach (var param in operation.Parameters)
        {
            if (param.Name == "api-version")
            {
                param.Description = "API Version";
                param.Required = false;
            }
        }

        operation.Deprecated |= context.ApiDescription.IsDeprecated();
    }
}
