using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.Errors;
using Nova.Friend.Domain.Exceptions.FriendRequest;
using Nova.Friend.Domain.FriendRequestAggregate.Enumerations;
using Nova.Friend.Domain.FriendRequestAggregate.Events;
using Nova.Friend.Domain.FriendRequestAggregate.ValueObjects;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Domain.FriendRequestAggregate;

public class FriendRequest : AggregateRoot<Id>
{
    private FriendRequest(
        Id id,
        Id senderId,
        Id receiverId,
        RequestStatus status)
    {
        SenderId = senderId;
        ReceiverId = receiverId;
        Status = status;
        Id = id;
    }
    
    public Id SenderId { get; }
    public Id ReceiverId { get; }
    public RequestStatus Status { get; private set; }

    public void Accept()
    {
        if (Status != RequestStatus.Pending)
            throw new AcceptedInvitationException(Id.Value);

        Status = RequestStatus.Accepted;
        RaiseDomainEvent(new AcceptedFriendRequestDomainEvent(Id.Value, SenderId.Value, ReceiverId.Value));
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
        var friendRequestIdResult = Id.Create(friendRequestId);
        if (friendRequestIdResult.IsFailure)
            return Result.Failure<FriendRequest>(FriendRequestError.InvalidFriendRequestId);

        var senderIdResult = Id.Create(senderId);
        if (senderIdResult.IsFailure)
            return Result.Failure<FriendRequest>(RelationShipError.InvalidSenderId);

        var receiverIdResult = Id.Create(receiverId);
        if (receiverIdResult.IsFailure)
            return Result.Failure<FriendRequest>(RelationShipError.InvalidReceiverId);

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