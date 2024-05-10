using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Domain.FriendRequestAggregate.Repositories;

public interface IFriendSearchRepository
{
    Task<bool> CheckExistFriendRequestBetweenUsers(UserId senderId, UserId receiverId, CancellationToken cancellationToken = default);
    Task<FriendRequest?> FindBySenderAndReceiver(UserId senderId, UserId receiverId, CancellationToken cancellationToken = default);
    Task<List<FriendRequest>?> GetFriendRequests(UserId receiverId, CancellationToken cancellationToken = default);
}