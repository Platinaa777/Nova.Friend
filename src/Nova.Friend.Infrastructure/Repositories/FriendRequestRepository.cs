using Core.Arango;
using Core.Arango.Linq;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.FriendRequestAggregate;
using Nova.Friend.Domain.FriendRequestAggregate.Repositories;
using Nova.Friend.Domain.FriendRequestAggregate.ValueObjects;
using Nova.Friend.Infrastructure.Snapshots;

namespace Nova.Friend.Infrastructure.Repositories;

public class FriendRequestRepository : IFriendRequestRepository
{
    private readonly IArangoContext _dbContext;
    private readonly ITransactionScope _scope;

    public FriendRequestRepository(
        IArangoContext dbContext,
        ITransactionScope scope)
    {
        _dbContext = dbContext;
        _scope = scope;
    }
    
    public async Task<FriendRequest?> FindFriendRequestById(RequestId requestId, CancellationToken cancellationToken = default)
    {
        var snapshot = await _dbContext
            .Query<FriendRequestSnapshot>(_scope.TransactionId)
            .FirstOrDefaultAsync(x => x.Id == requestId.Value, cancellationToken);

        if (snapshot is null)
            return null;
        
        return snapshot.Restore();
    }

    public async Task Add(FriendRequest friendRequest, CancellationToken cancellationToken = default)
    {
        var snapshot = friendRequest.Save();

        await _dbContext.Document.CreateAsync(_scope.TransactionId, DatabaseOptions.RequestCollection, snapshot,
            cancellationToken: cancellationToken);
    }

    public async Task Update(FriendRequest friendRequest, CancellationToken cancellationToken = default)
    {
        var snapshot = friendRequest.Save();

        await _dbContext.Document.UpdateAsync(_scope.TransactionId, DatabaseOptions.RequestCollection, snapshot,
            cancellationToken: cancellationToken);
    }
}