using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Timpra.API.Swagger;

public static class CustomSwaggerMiddleware
{
    public static void UseCustomSwaggerUI(this WebApplication? app)
    {
        if( app is not null)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        $"Version {description.GroupName.ToUpper()}");
                }
            });
        }
    }
}
