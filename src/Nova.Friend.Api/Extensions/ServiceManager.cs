using Nova.Friend.Api.BackgroundJobs;
using Nova.Friend.Application.Monitoring;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quartz;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace Nova.Friend.Api.Extensions;

public static class ServiceManager
{
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddQuartz(cfg =>
        {
            var key = new JobKey(nameof(OutboxMessageJob));

            cfg.SchedulerName = Guid.NewGuid().ToString();
            
            cfg.AddJob<OutboxMessageJob>(key)
                .AddTrigger(tg => 
                    tg.ForJob(key)
                        .WithSimpleSchedule(schedule => 
                            schedule.WithIntervalInSeconds(10)
                                .RepeatForever()));
            
        });
        
        services.AddQuartzHostedService();

        return services;
    }
    
    
    public static IServiceCollection AddLogging(this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment) =>
            services.AddLogging(b => b.AddSerilog(new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithProperty("App", ApplicationMetrics.ApplicationName)
                .WriteTo.Logger(logCfg => 
                    logCfg.WriteTo.Console()
                        .WriteTo.GrafanaLoki(configuration.GetConnectionString("Loki")!,
                            propertiesAsLabels: new []
                            {
                                "Environment", "level", "App", "SourceContext"
                            },
                            leavePropertiesIntact: true)
                        .Enrich.WithProperty("service_name", ApplicationMetrics.ApplicationName))
                .CreateLogger()));

    public static IServiceCollection AddMetrics(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithMetrics(cfg => cfg
                .AddMeter(ApplicationMetrics.ApplicationName)
                .AddAspNetCoreInstrumentation()
                .AddPrometheusExporter())
            .WithTracing(b => b
                .ConfigureResource(r => r.AddService("Nova"))
                .AddAspNetCoreInstrumentation(opt =>
                {
                    opt
                        .Filter += ctx =>
                        !ctx.Request.Path.Value!.Contains("metrics",
                            StringComparison.InvariantCultureIgnoreCase) && // for prometheus
                        !ctx.Request.Path.Value!.Contains("swagger", StringComparison.InvariantCultureIgnoreCase); // for swagger
                    opt.EnrichWithHttpResponse = (activity, res) =>
                        activity.AddTag("error", res.StatusCode >= 400);
                })
                .AddHttpClientInstrumentation()
                .AddSource(ApplicationMetrics.ApplicationName)
                .AddConsoleExporter()
                .AddJaegerExporter(opt => 
                    opt.Endpoint = new Uri(configuration.GetConnectionString("Jaeger")!)));
        
        return services;
    }
}