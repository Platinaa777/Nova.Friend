namespace Nova.Friend.Domain.Exceptions.FriendShipInvitation;

public class CanceledFriendRequestException : DomainException
{
    public CanceledFriendRequestException(string invitationId)
        : base($"FriendRequest with id {invitationId} should be in pending status to be canceled")
    {
    }
}