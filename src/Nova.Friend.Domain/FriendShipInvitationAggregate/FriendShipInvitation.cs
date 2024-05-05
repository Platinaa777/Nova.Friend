using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.Exceptions.FriendShipInvitation;
using Nova.Friend.Domain.FriendShipInvitationAggregate.Enumerations;
using Nova.Friend.Domain.FriendShipInvitationAggregate.Events;
using Nova.Friend.Domain.FriendShipInvitationAggregate.ValueObjects;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Domain.FriendShipInvitationAggregate;

public class FriendShipInvitation : AggregateRoot<InvitationId>
{
    private FriendShipInvitation(
        InvitationId id,
        UserId senderId,
        UserId receiverId,
        InvitationStatus status)
    {
        SenderId = senderId;
        ReceiverId = receiverId;
        Status = status;
        Id = id;
    }
    
    public UserId SenderId { get; }
    public UserId ReceiverId { get; }
    public InvitationStatus Status { get; private set; }

    public void Accept()
    {
        if (Status != InvitationStatus.Pending)
            throw new AcceptedInvitationException(Id.Value);

        Status = InvitationStatus.Accepted;
        RaiseDomainEvent(new AcceptedInvitationDomainEvent(Id, SenderId, ReceiverId));
    }

    public void Reject()
    {
        if (Status != InvitationStatus.Pending)
            throw new RejectedInvitationException(Id.Value);

        Status = InvitationStatus.Rejected;
    }

    public void Cancel()
    {
        if (Status != InvitationStatus.Pending)
            throw new CanceledInvitationException(Id.Value);

        Status = InvitationStatus.Canceled;
    }

    public static Result<FriendShipInvitation> Create(
        string invitationId,
        string senderId,
        string receiverId,
        string? status)
    {
        var invitationIdResult = InvitationId.Create(invitationId);
        if (invitationIdResult.IsFailure)
            return Result.Failure<FriendShipInvitation>(invitationIdResult.Error);

        var senderIdResult = UserId.Create(senderId);
        if (senderIdResult.IsFailure)
            return Result.Failure<FriendShipInvitation>(senderIdResult.Error);

        var receiverIdResult = UserId.Create(receiverId);
        if (receiverIdResult.IsFailure)
            return Result.Failure<FriendShipInvitation>(receiverIdResult.Error);

        if (status is null)
            return Result.Failure<FriendShipInvitation>(InvitationError.EmptyStatusType);
        
        var statusResult = InvitationStatus.FromName(status);
        if (statusResult is null)
            return Result.Failure<FriendShipInvitation>(InvitationError.NotExistingStatusType);

        return new FriendShipInvitation(
            invitationIdResult.Value,
            senderIdResult.Value,
            receiverIdResult.Value,
            statusResult);
    }
}