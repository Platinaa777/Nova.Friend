using Core.Arango;
using Core.Arango.Linq;
using DomainDrivenDesign.Abstractions;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.UserAggregate;
using Nova.Friend.Domain.UserAggregate.Repositories;
using Nova.Friend.Infrastructure.Persistence.Abstractions;
using Nova.Friend.Infrastructure.Snapshots;

namespace Nova.Friend.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IArangoContext _dbContext;
    private readonly ITransactionScope _scope;
    private readonly IChangeTracker _tracker;

    public UserRepository(
        IArangoContext dbContext,
        ITransactionScope scope,
        IChangeTracker tracker)
    {
        _dbContext = dbContext;
        _scope = scope;
        _tracker = tracker;
    }
    
    public async Task<User?> FindUserById(Id userId, CancellationToken cancellationToken = default)
    {
        UserSnapshot snapshot = await _dbContext.Query<UserSnapshot>(_scope.TransactionId)
                .FirstOrDefaultAsync(x => x.Key == userId.Value, cancellationToken);
    
        if (snapshot is null)
            return null;

        var fromUser = DatabaseOptions.UserCollection + "/" + snapshot.Key;

        var friends = await _dbContext.Query.ExecuteAsync<UserSnapshot>(
            _scope.TransactionId,
            $@"
                FOR v, e IN 1..1 ANY {fromUser} GRAPH 'Relations'
                FILTER e.Label == 'friends'
                RETURN v", cancellationToken: cancellationToken);

        List<string> friendIds = new();

        if (friends is not null)
        {
            foreach (var friendSnapshot in friends)
                friendIds.Add(friendSnapshot.Key);
        }

        snapshot.FriendIds = friendIds;
        
        var user = snapshot.Restore();
        
        _tracker.Track(user);

        return user;
    }

    public async Task Add(User user, CancellationToken cancellationToken = default)
    {
        var snapshot = user.Save();

        await _dbContext.Graph.Vertex.CreateAsync(
            _scope.TransactionId,
            DatabaseOptions.RelationGraph,
            DatabaseOptions.UserCollection,
            snapshot,
            cancellationToken: cancellationToken);
    }

    public async Task Update(User user, CancellationToken cancellationToken = default)
    {
        var snapshot = user.Save();
        
        await _dbContext.Graph.Vertex.UpdateAsync(
            _scope.TransactionId,
            DatabaseOptions.RelationGraph,
            DatabaseOptions.UserCollection,
            snapshot.Key,
            snapshot,
            cancellationToken: cancellationToken);
    }
}