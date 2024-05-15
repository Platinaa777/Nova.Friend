using Nova.Friend.Api.BackgroundJobs;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using Quartz;
using Serilog;
using Serilog.Events;

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
                .MinimumLevel.Debug()
                .Enrich.WithProperty("Api", "Nova.Friend.Api")
                .WriteTo.Logger(logCfg => 
                    logCfg.WriteTo.Console())
                .CreateLogger()));

    public static IServiceCollection AddMetrics(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(cfg =>
                cfg
                    .AddAspNetCoreInstrumentation()
                    .AddConsoleExporter()
                    .AddPrometheusExporter());
        
        return services;
    }
}