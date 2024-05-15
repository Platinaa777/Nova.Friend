using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Quartz.Logging;

namespace Nova.Friend.IntegrationTests;

public class BaseIntegrationTest : IClassFixture<IntegrationTestingWebAppFactory>
{
    protected readonly IntegrationTestingWebAppFactory _factory;
    protected readonly IServiceScope _scope;
    protected readonly IMediator _mediator;
    
    public BaseIntegrationTest(IntegrationTestingWebAppFactory factory)
    {
        _factory = factory;
        _scope = factory.Services.CreateScope();

        _mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
    }
}