using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace SwaggerConfiguration;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSingleton<IStartupFilter, SwaggerFilter>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo() { 
                Title = $"{Assembly.GetEntryAssembly()?.GetName().Name ?? "no name"}.v1",
                Version = "v1" 
            });
            opt.CustomSchemaIds(x => x.FullName);

            // Add JWT Authentication options
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    securityScheme,
                    new string[] { }
                }
            });
        });    
        
        return services;
    }    
}