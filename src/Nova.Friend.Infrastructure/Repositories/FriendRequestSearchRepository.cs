using Core.Arango;
using Core.Arango.Linq;
using DomainDrivenDesign.Abstractions;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.FriendRequestAggregate;
using Nova.Friend.Domain.FriendRequestAggregate.Enumerations;
using Nova.Friend.Domain.FriendRequestAggregate.Repositories;
using Nova.Friend.Infrastructure.Persistence.Abstractions;
using Nova.Friend.Infrastructure.Snapshots;

namespace Nova.Friend.Infrastructure.Repositories;

public class FriendRequestSearchRepository : IFriendSearchRepository
{
    private readonly IArangoContext _dbContext;
    private readonly ITransactionScope _scope;
    private readonly IChangeTracker _tracker;

    public FriendRequestSearchRepository(
        IArangoContext dbContext,
        ITransactionScope scope,
        IChangeTracker tracker)
    {
        _dbContext = dbContext;
        _scope = scope;
        _tracker = tracker;
    }
    
    public async Task<bool> CheckExistFriendRequestBetweenUsers(Id senderId, Id receiverId, CancellationToken cancellationToken = default)
    {
        try {
            var res = await _dbContext
                .Query<FriendRequestSnapshot>(_scope.TransactionId)
                .Where(x => x.SenderId == senderId.Value
                            && x.ReceiverId == receiverId.Value
                            && x.Status == RequestStatus.Pending.Name)
                .ToListAsync(cancellationToken);

            return !(res is null || res.Count == 0);
        }
        catch (Exception)
        {
            // ignored
        }
        return false;
    }

    public async Task<FriendRequest?> FindBySenderAndReceiver(Id senderId, Id receiverId, CancellationToken cancellationToken = default)
    {
        try
        {
            var res = await _dbContext
                .Query<FriendRequestSnapshot>(_scope.TransactionId)
                .Where(x => x.SenderId == senderId.Value
                            && x.ReceiverId == receiverId.Value
                            && x.Status == RequestStatus.Pending.Name)
                .ToListAsync(cancellationToken);

            if (res is null || res.Count == 0)
                return null;

            var snapshot = res.FirstOrDefault()!;
            
            var fr = snapshot.Restore();

            _tracker.Track(fr);
            
            return fr;
        }
        catch (Exception)
        {
            // ignored
        }

        return null;
    }

    public async Task<List<FriendRequest>?> GetFriendRequests(Id receiverId, CancellationToken cancellationToken = default)
    {
        try
        {
            var res = await _dbContext
                .Query<FriendRequestSnapshot>(_scope.TransactionId)
                .Where(x => x.ReceiverId == receiverId.Value 
                            && x.Status == RequestStatus.Pending.Name)
                .ToListAsync(cancellationToken);

            if (res is null || res.Count == 0)
                return new();

            List<FriendRequest> list = new();
            foreach (var snapshot in res)
            {
                var fr = snapshot.Restore();
                _tracker.Track(fr);
                list.Add(fr);
            }
            
            return list;
        }
        catch (Exception)
        {
            // ignored
        }
        return new();
    }
}