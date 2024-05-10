using Nova.Friend.Api.BackgroundJobs;
using Quartz;

namespace Nova.Friend.Api.Extensions;

public static class ServiceManager
{
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddQuartz(cfg =>
        {
            var key = new JobKey(nameof(OutboxMessageJob));
        
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
}