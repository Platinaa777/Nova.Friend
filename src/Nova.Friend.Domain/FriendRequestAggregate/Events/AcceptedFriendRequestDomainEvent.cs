using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.FriendShipInvitationAggregate.ValueObjects;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Domain.FriendShipInvitationAggregate.Events;

public record AcceptedFriendRequestDomainEvent(RequestId RequestId, UserId SenderId, UserId ReceiverId)
    : IDomainEvent; 