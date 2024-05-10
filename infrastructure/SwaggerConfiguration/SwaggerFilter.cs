using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SwaggerConfiguration;

public class SwaggerFilter : IStartupFilter
{
    private readonly IWebHostEnvironment _env;
    
    public SwaggerFilter(IWebHostEnvironment env)
    {
        _env = env;
    }
    
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            next(app);
        };
    }
}