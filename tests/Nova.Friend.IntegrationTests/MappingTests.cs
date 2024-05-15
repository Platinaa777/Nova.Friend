using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Nova.Friend.IntegrationTests;

public class MappingTests : BaseIntegrationTest
{
    public MappingTests(IntegrationTestingWebAppFactory factory) : base(factory)
    {
    }
    
    [Fact]
    public void AutoMapper_Configuration_IsValid()
    {
        var configurationProvider = _factory.Services.GetRequiredService<IMapper>().ConfigurationProvider;
        configurationProvider.Invoking(p => p.AssertConfigurationIsValid())
            .Should().NotThrow();
    }
}