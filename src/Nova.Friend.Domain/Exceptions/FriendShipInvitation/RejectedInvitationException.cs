namespace Nova.Friend.Domain.Exceptions.FriendShipInvitation;

public class RejectedInvitationException : DomainException
{
    public RejectedInvitationException(string invitationId)
        : base($"Invitation with id {invitationId} should be in pending status to be rejected")
    {
    }
}