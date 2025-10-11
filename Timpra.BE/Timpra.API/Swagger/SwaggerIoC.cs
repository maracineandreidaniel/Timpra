using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Timpra.API.Swagger;

public static class SwaggerIoC
{
    public static void AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOptions>();
    }
}
