using System.Reflection;
using Core.Arango;
using DomainDrivenDesign.Abstractions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nova.Friend.Application.Assembly;
using Nova.Friend.Application.Behavior;
using Nova.Friend.Application.Commands.CreateUser;
using Nova.Friend.Application.Factories;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.FriendRequestAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.Repositories;
using Nova.Friend.Infrastructure.Assembly;
using Nova.Friend.Infrastructure.Persistence;
using Nova.Friend.Infrastructure.Persistence.Abstractions;
using Nova.Friend.Infrastructure.Repositories;
using IUnitOfWork = Nova.Friend.Application.TransactionScope.IUnitOfWork;

namespace Nova.Friend.DependencyInjection;

public static class ServiceCollectionRegister
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<CreateUserCommandHandler>());

        services
            .AddScoped<IFriendRequestRepository, FriendRequestRepository>()
            .AddScoped<IFriendSearchRepository, FriendRequestSearchRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IIdGeneratorFactory<Guid>, GuidFactory>();
        
        services
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        services.AddValidatorsFromAssembly(
            ApplicationAssembly.Assembly,
            includeInternalTypes: true);

        services.AddAutoMapper(config => 
            config.AddMaps(Assembly.GetEntryAssembly()));
        
        return services;
    }
    
    public static IServiceCollection AddDataLayer(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddArango((sp, cfg) =>
        {
            cfg.ConnectionString = configuration.GetConnectionString("Arango");
        });
        
        services.AddAutoMapper(InfrastructureAssembly.Assembly);

        services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IChangeTracker<ValueObject>, ChangeTracker>()
            .AddScoped<ITransactionScope, ArangoTransactionScope>();
        
        return services;
    }
}