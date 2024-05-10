using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.FriendRequestAggregate.ValueObjects;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Domain.FriendRequestAggregate.Events;

public record AcceptedFriendRequestDomainEvent(RequestId RequestId, UserId SenderId, UserId ReceiverId)
    : IDomainEvent; 
