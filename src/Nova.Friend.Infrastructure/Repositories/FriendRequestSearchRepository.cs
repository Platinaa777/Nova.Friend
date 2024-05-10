using Nova.Friend.Domain.FriendRequestAggregate;
using Nova.Friend.Domain.FriendRequestAggregate.Repositories;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Infrastructure.Repositories;

public class FriendRequestSearchRepository : IFriendSearchRepository
{
    public Task<bool> FindFriendRequestBetweenUsers(UserId senderId, UserId receiverId)
    {
        throw new NotImplementedException();
    }

    public Task<FriendRequest?> FindBySenderAndReceiver(UserId senderId, UserId receiverId)
    {
        throw new NotImplementedException();
    }
}