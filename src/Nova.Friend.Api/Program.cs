using Nova.Friend.Api.Extensions;
using Nova.Friend.DependencyInjection;
using SwaggerConfiguration;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddApplicationServices()
    .AddDataLayer(configuration)
    .AddBackgroundJobs();

builder.Services
    .AddSwagger();

var app = builder.Build();

app.MapControllers();

app.Run();