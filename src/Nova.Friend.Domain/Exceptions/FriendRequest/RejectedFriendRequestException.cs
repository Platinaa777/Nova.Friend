namespace Nova.Friend.Domain.Exceptions.FriendRequest;

public class RejectedFriendRequestException : DomainException
{
    public RejectedFriendRequestException(string invitationId)
        : base($"FriendRequest with id {invitationId} should be in pending status to be rejected")
    {
    }
}