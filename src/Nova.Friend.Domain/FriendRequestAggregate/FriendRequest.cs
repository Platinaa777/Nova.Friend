using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.Exceptions.FriendShipInvitation;
using Nova.Friend.Domain.FriendShipInvitationAggregate.Enumerations;
using Nova.Friend.Domain.FriendShipInvitationAggregate.Events;
using Nova.Friend.Domain.FriendShipInvitationAggregate.ValueObjects;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Domain.FriendShipInvitationAggregate;

public class FriendRequest : AggregateRoot<RequestId>
{
    private FriendRequest(
        RequestId id,
        UserId senderId,
        UserId receiverId,
        RequestStatus status)
    {
        SenderId = senderId;
        ReceiverId = receiverId;
        Status = status;
        Id = id;
    }
    
    public UserId SenderId { get; }
    public UserId ReceiverId { get; }
    public RequestStatus Status { get; private set; }

    public void Accept()
    {
        if (Status != RequestStatus.Pending)
            throw new AcceptedInvitationException(Id.Value);

        Status = RequestStatus.Accepted;
        RaiseDomainEvent(new AcceptedFriendRequestDomainEvent(Id, SenderId, ReceiverId));
    }

    public void Reject()
    {
        if (Status != RequestStatus.Pending)
            throw new RejectedFriendRequestException(Id.Value);

        Status = RequestStatus.Rejected;
    }

    public void Cancel()
    {
        if (Status != RequestStatus.Pending)
            throw new CanceledFriendRequestException(Id.Value);

        Status = RequestStatus.Canceled;
    }

    public static Result<FriendRequest> Create(
        string friendRequestId,
        string senderId,
        string receiverId,
        string? status)
    {
        var friendRequestIdResult = RequestId.Create(friendRequestId);
        if (friendRequestIdResult.IsFailure)
            return Result.Failure<FriendRequest>(friendRequestIdResult.Error);

        var senderIdResult = UserId.Create(senderId);
        if (senderIdResult.IsFailure)
            return Result.Failure<FriendRequest>(senderIdResult.Error);

        var receiverIdResult = UserId.Create(receiverId);
        if (receiverIdResult.IsFailure)
            return Result.Failure<FriendRequest>(receiverIdResult.Error);

        if (status is null)
            return Result.Failure<FriendRequest>(FriendRequestError.EmptyStatusType);
        
        var statusResult = RequestStatus.FromName(status);
        if (statusResult is null)
            return Result.Failure<FriendRequest>(FriendRequestError.NotExistingStatusType);

        return new FriendRequest(
            friendRequestIdResult.Value,
            senderIdResult.Value,
            receiverIdResult.Value,
            statusResult);
    }
}