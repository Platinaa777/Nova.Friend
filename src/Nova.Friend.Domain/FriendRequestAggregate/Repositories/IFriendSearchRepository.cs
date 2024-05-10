using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Domain.FriendShipInvitationAggregate.Repositories;

public interface IFriendSearchRepository
{
    Task<bool> FindFriendRequestBetweenUsers(UserId senderId, UserId receiverId);
    Task<FriendRequest?> FindBySenderAndReceiver(UserId senderId, UserId receiverId);
}