using Core.Arango;
using Core.Arango.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Nova.Friend.Application.Commands.CreateUser;
using Nova.Friend.Application.Constants;
using Nova.Friend.Domain.UserAggregate.ValueObjects;
using Nova.Friend.Infrastructure.Snapshots;
using Xunit.Abstractions;

namespace Nova.Friend.IntegrationTests;

public class CreateUserCommandHandlerShould : BaseIntegrationTest
{
    [Fact]
    public async Task Create_WhenAllValid()
    {
        var id = UserId.Create("84739750-94F0-4D43-98DA-5C0D576E8A6A").Value;
        
        var command = new CreateUserCommand
        {
            UserId = id.Value,
            UserName = "denis",
            LastName = "adminovich"
        };

        var result = await _mediator.Send(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var arango = _scope.ServiceProvider.GetRequiredService<IArangoContext>();
        
        var user = await arango.Query<UserSnapshot>(DatabaseOptions.DatabaseName)
            .FirstOrDefaultAsync(x => x.Key == id.Value, CancellationToken.None);
        Assert.NotNull(user);
        user.Key.Should().Be(id.Value);
    }
    
    public CreateUserCommandHandlerShould(IntegrationTestingWebAppFactory factory) : base(factory)
    {
    }
}