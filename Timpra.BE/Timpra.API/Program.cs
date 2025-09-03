using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Timpra.API.Middleware;
using Timpra.BusinessLogic.Helpers.TokenAuthentication;
using Timpra.BusinessLogic.Services;
using Timpra.BusinessLogic.Services.Abstractions;
using Timpra.DataAccess.Context;
using Timpra.DataAccess.Repository;
using Timpra.DataAccess.Repository.Abstraction;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddTransient<ITokenManager, TokenManager>();

        builder.Services.AddCors(options => options.AddPolicy(name: "AllowAny", builder =>
        {
            builder.AllowAnyOrigin();
            builder.AllowAnyMethod();
            builder.AllowAnyHeader();
        }));

        builder.Services.AddControllers();

        builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("dbConnectionString")));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IAuthenticateService, AuthenticateService>();
        builder.Services.AddScoped<IOrderService, OrderService>();

        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new() { Title = "Timpra.API", Version = "v1" });
            option.AddSecurityDefinition(
               "Bearer",
               new OpenApiSecurityScheme
               {
                   In = ParameterLocation.Header,
                   Description = "Please enter a valid token -> Bearer {token}",
                   Name = "Authorization",
                   Type = SecuritySchemeType.Http,
                   BearerFormat = "JWT",
                   Scheme = "Bearer"
               }
           );
            option.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
                }
            );
        });

        var app = builder.Build();

        app.UseOptions();

        app.UseCors("AllowAny");

        // Configure the HTTP request pipeline.
        if (builder.Environment.IsDevelopment() || builder.Environment.IsProduction())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Timpra.API v1"));
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }
}