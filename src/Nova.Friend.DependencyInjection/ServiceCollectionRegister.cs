using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nova.Friend.Application.Behavior;
using Nova.Friend.Application.Commands.CreateUser;

namespace Nova.Friend.DependencyInjection;

public static class ServiceCollectionRegister
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<CreateUserCommandHandler>());
        
        services
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        return services;
    }
}