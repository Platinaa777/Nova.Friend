using Core.Arango;
using Core.Arango.Linq;
using DomainDrivenDesign.Abstractions;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.FriendRequestAggregate;
using Nova.Friend.Domain.FriendRequestAggregate.Repositories;
using Nova.Friend.Infrastructure.Persistence.Abstractions;
using Nova.Friend.Infrastructure.Snapshots;

namespace Nova.Friend.Infrastructure.Repositories;

public class FriendRequestRepository : IFriendRequestRepository
{
    private readonly IArangoContext _dbContext;
    private readonly ITransactionScope _scope;
    private readonly IChangeTracker _tracker;

    public FriendRequestRepository(
        IArangoContext dbContext,
        ITransactionScope scope,
        IChangeTracker tracker)
    {
        _dbContext = dbContext;
        _scope = scope;
        _tracker = tracker;
    }
    
    public async Task<FriendRequest?> FindFriendRequestById(Id requestId, CancellationToken cancellationToken = default)
    {
        var snapshot = await _dbContext
            .Query<FriendRequestSnapshot>(_scope.TransactionId)
            .FirstOrDefaultAsync(x => x.Key == requestId.Value, cancellationToken);

        if (snapshot is null)
            return null;

        var fr = snapshot.Restore();
        
        _tracker.Track(fr);

        return fr;
    }

    public async Task Add(FriendRequest friendRequest, CancellationToken cancellationToken = default)
    {
        var snapshot = friendRequest.Save();

        await _dbContext.Document.CreateAsync(
            _scope.TransactionId,
            DatabaseOptions.RequestCollection,
            snapshot,
            cancellationToken: cancellationToken);
    }

    public async Task Update(FriendRequest friendRequest, CancellationToken cancellationToken = default)
    {
        var snapshot = friendRequest.Save();

        await _dbContext.Document.UpdateAsync(
            _scope.TransactionId,
            DatabaseOptions.RequestCollection,
            snapshot,
            cancellationToken: cancellationToken);
    }
}