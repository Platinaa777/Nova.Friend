using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Domain.UserAggregate.Events;

public record RemovedFriendDomainEvent(UserId FriendRemovalInitiator, UserId RemovedFriend) : IDomainEvent;
