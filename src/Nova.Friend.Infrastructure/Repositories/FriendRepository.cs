using Core.Arango;
using DomainDrivenDesign.Abstractions;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.UserAggregate.Repositories;
using Nova.Friend.Infrastructure.Models;

namespace Nova.Friend.Infrastructure.Repositories;

public class FriendRepository : IFriendRepository
{
    private readonly IArangoContext _dbContext;
    private readonly ITransactionScope _scope;

    public FriendRepository(
        IArangoContext dbContext,
        ITransactionScope scope)
    {
        _dbContext = dbContext;
        _scope = scope;
    }
    
    public async Task DeleteFriend(Id from, Id to)
    {
        var key = CalculateKey(from.Value, to.Value);
        
        await _dbContext.Graph.Edge.RemoveAsync<FriendEdge>(
            _scope.TransactionId,
            DatabaseOptions.RelationGraph,
            DatabaseOptions.FriendEdge,
            key);
    }

    public async Task AddFriend(Id from, Id to)
    {
        var key = CalculateKey(from.Value, to.Value);
        
        await _dbContext.Graph.Edge.CreateAsync(_scope.TransactionId, DatabaseOptions.RelationGraph,
            DatabaseOptions.FriendEdge, new FriendEdge
            {
                Key = key,
                From = DatabaseOptions.UserCollection + "/" + from.Value,
                To = DatabaseOptions.UserCollection + "/" + to.Value,
                Label = "friends"
            });
    }

    private string CalculateKey(string from, string to)
    {
        List<string> ids = new List<string> { from, to };
        var sortedIds = ids.OrderBy(id => id).ToList();
        var (fromUser, toUser) = (sortedIds[0], sortedIds[1]);
        
        return HashCode.Combine(Guid.Parse(fromUser), Guid.Parse(toUser)).ToString();
    }
}