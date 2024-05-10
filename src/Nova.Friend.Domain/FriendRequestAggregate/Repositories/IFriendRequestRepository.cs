using Nova.Friend.Domain.FriendRequestAggregate.ValueObjects;

namespace Nova.Friend.Domain.FriendRequestAggregate.Repositories;

public interface IFriendRequestRepository
{
    Task<FriendRequest?> FindFriendRequestById(RequestId requestId, CancellationToken cancellationToken = default);
    Task Add(FriendRequest friendRequest, CancellationToken cancellationToken = default);
    Task Update(FriendRequest friendRequest, CancellationToken cancellationToken = default);
}