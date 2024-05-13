using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Domain.FriendRequestAggregate.Repositories;

public interface IFriendSearchRepository
{
    Task<bool> CheckExistFriendRequestBetweenUsers(Id senderId, Id receiverId, CancellationToken cancellationToken = default);
    Task<FriendRequest?> FindBySenderAndReceiver(Id senderId, Id receiverId, CancellationToken cancellationToken = default);
    Task<List<FriendRequest>?> GetFriendRequests(Id receiverId, CancellationToken cancellationToken = default);
}