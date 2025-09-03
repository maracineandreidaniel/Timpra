using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Timpra.BusinessLogic.DTOs.Orders;
using Timpra.BusinessLogic.Services.Abstractions;
using Timpra.DataAccess.Context;
using Timpra.DataAccess.Entities;
using NSubstitute;

namespace Timpra.API.Test.Utilities;

public class CustomEndpointContractTestFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var serializerOptions = new DefaultJsonTypeInfoResolver
        {
            Modifiers =
            {
                static typeInfo =>
                {
                    if (typeInfo.Kind != JsonTypeInfoKind.Object)
                    {
                        return;
                    }

                    foreach (var propertyInfo in typeInfo.Properties)
                    {
                        var hasJsonIgnoreProperty = propertyInfo.AttributeProvider != null &&
                                                    propertyInfo.AttributeProvider.GetCustomAttributes(false).OfType<JsonIgnoreAttribute>().Any();

                        if (hasJsonIgnoreProperty)
                        {
                            continue;
                        }

                        if (!propertyInfo.IsExtensionData)
                        {
                            propertyInfo.IsRequired = true;
                        }
                    }
                }
            }
        };

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication("FakeScheme")
                .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>("FakeScheme", options => { });

            services.RemoveAll<AppDbContext>();
            services.RemoveAll<IOrderService>();
            services.RemoveAll<IAuthenticateService>();

            var authServiceMock = Substitute.For<IAuthenticateService>();
            authServiceMock.Login(Arg.Any<LoginDTO>()).Returns(new User { Id = 1, Username = "string", Password = "string", FullName = "string" });

            services.AddSingleton(_ => Substitute.For<AppDbContext>());
            services.AddSingleton(_ => Substitute.For<IOrderService>());
            services.AddScoped(_ => authServiceMock);

            services.AddControllers();
            services.AddCors(options => options.AddPolicy("AllowAny", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.PostConfigure<JsonOptions>(opt =>
            {
                // The tests will send all possible properties to the server (even optional ones).
                // Setting unmapped members to disallow ensures us that even the optional properties are set according to the input
                opt.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;

                // Make all the properties required so even optional properties are validated to be filled in
                opt.JsonSerializerOptions.TypeInfoResolver = serializerOptions;

                // Make the serializer case insensitive
                opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
        });

        base.ConfigureWebHost(builder);
    }
}
