using Nova.Friend.Api.Extensions;
using Nova.Friend.DependencyInjection;
using SwaggerConfiguration;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var environment = builder.Environment;

builder.Services
    .AddApplicationServices()
    .AddDataLayer(configuration)
    .AddBackgroundJobs()
    .AddLogging(configuration, environment)
    .AddMetrics();

builder.Services
    .AddSwagger();

var app = builder.Build();

app.MapControllers();
app.MapPrometheusScrapingEndpoint();

app.Run();

public partial class Program
{
}