using Nova.Friend.Domain.FriendShipInvitationAggregate.ValueObjects;

namespace Nova.Friend.Domain.FriendShipInvitationAggregate.Repositories;

public interface IFriendRequestRepository
{
    Task<FriendRequest?> FindFriendRequestById(RequestId requestId, CancellationToken cancellationToken = default);
    Task Add(FriendRequest friendRequest, CancellationToken cancellationToken = default);
    Task Update(FriendRequest friendRequest, CancellationToken cancellationToken = default);
}