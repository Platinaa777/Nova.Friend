using Core.Arango;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.Queries.GetFriends;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Infrastructure.Snapshots;
using Quartz.Logging;

namespace Nova.Friend.IntegrationTests;

public class GetFriendsQueryHandlerShould : BaseIntegrationTest
{
    public GetFriendsQueryHandlerShould(IntegrationTestingWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ReturnError_WhenUserNotExist()
    {
        var query = new GetFriendsQuery() { UserId = "3F1CE25A-CEB5-41C3-9A99-2371CD762A20" };

        var result = await _mediator.Send(query, CancellationToken.None);
        
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserError.NotFound);
    }
    
    [Fact]
    public async Task ReturnSuccess_WhenUserExist()
    {
        var arango = _scope.ServiceProvider.GetRequiredService<IArangoContext>();
        var id = "C546BE2D-164E-4EFC-A3AC-4714A3DC3770";
        var r = await arango.Document.CreateAsync<UserSnapshot>(
            DatabaseOptions.DatabaseName,
            DatabaseOptions.UserCollection,
            new()
            {
                Id = id,
                Key = id,
                FirstName = "den",
                LastName = "admin",
                FriendIds = new()
            });

        var a = await arango.Document.GetAsync<UserSnapshot>(DatabaseOptions.DatabaseName, DatabaseOptions.UserCollection, id);
        
        var query = new GetFriendsQuery() { UserId = id };

        var result = await _mediator.Send(query, CancellationToken.None);
        
        result.IsSuccess.Should().BeTrue();
    }
}