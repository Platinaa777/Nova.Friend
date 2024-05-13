using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Domain.UserAggregate.Repositories;

public interface IFriendRepository
{
    Task DeleteFriend(Id from, Id to);
    Task AddFriend(Id from, Id to);
}