using Core.Arango;
using Core.Arango.Linq;
using Nova.Friend.Application.Constants;
using Nova.Friend.Application.TransactionScope;
using Nova.Friend.Domain.FriendRequestAggregate;
using Nova.Friend.Domain.FriendRequestAggregate.Enumerations;
using Nova.Friend.Domain.FriendRequestAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;
using Nova.Friend.Infrastructure.Snapshots;

namespace Nova.Friend.Infrastructure.Repositories;

public class FriendRequestSearchRepository : IFriendSearchRepository
{
    private readonly IArangoContext _dbContext;
    private readonly ITransactionScope _scope;

    public FriendRequestSearchRepository(
        IArangoContext dbContext,
        ITransactionScope scope)
    {
        _dbContext = dbContext;
        _scope = scope;
    }
    
    public async Task<bool> CheckExistFriendRequestBetweenUsers(UserId senderId, UserId receiverId, CancellationToken cancellationToken = default)
    {
        try {
            var res = await _dbContext
                .Query<FriendRequestSnapshot>(_scope.TransactionId)
                .Where(x => x.SenderId == senderId.Value && x.ReceiverId == receiverId.Value)
                .ToListAsync(cancellationToken);

            return !(res is null || res.Count == 0);
        }
        catch (Exception)
        {
            // ignored
        }
        return false;
    }

    public async Task<FriendRequest?> FindBySenderAndReceiver(UserId senderId, UserId receiverId, CancellationToken cancellationToken = default)
    {
        try
        {
            var res = await _dbContext
                .Query<FriendRequestSnapshot>(_scope.TransactionId)
                .Where(x => x.SenderId == senderId.Value && x.ReceiverId == receiverId.Value)
                .ToListAsync(cancellationToken);

            if (res is null || res.Count == 0)
                return null;

            var snapshot = res.FirstOrDefault()!;
            
            return snapshot.Restore();
        }
        catch (Exception)
        {
            // ignored
        }

        return null;
    }

    public async Task<List<FriendRequest>?> GetFriendRequests(UserId receiverId, CancellationToken cancellationToken = default)
    {
        try
        {
            var res = await _dbContext
                .Query<FriendRequestSnapshot>(_scope.TransactionId)
                .Where(x => x.ReceiverId == receiverId.Value && x.Status == RequestStatus.Pending.Name)
                .ToListAsync(cancellationToken);

            if (res is null || res.Count == 0)
                return new();

            List<FriendRequest> list = new();
            foreach (var snapshot in res)
            {
                list.Add(snapshot.Restore());
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