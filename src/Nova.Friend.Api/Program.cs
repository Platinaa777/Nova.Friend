using Nova.Friend.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices();

var app = builder.Build();

app.Run();