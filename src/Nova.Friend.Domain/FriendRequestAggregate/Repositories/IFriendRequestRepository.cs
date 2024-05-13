using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Domain.FriendRequestAggregate.Repositories;

public interface IFriendRequestRepository
{
    Task<FriendRequest?> FindFriendRequestById(Id requestId, CancellationToken cancellationToken = default);
    Task Add(FriendRequest friendRequest, CancellationToken cancellationToken = default);
    Task Update(FriendRequest friendRequest, CancellationToken cancellationToken = default);
}