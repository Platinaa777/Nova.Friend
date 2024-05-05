namespace Nova.Friend.Domain.Exceptions.FriendShipInvitation;

public class CanceledInvitationException : DomainException
{
    public CanceledInvitationException(string invitationId)
        : base($"Invitation with id {invitationId} should be in pending status to be canceled")
    {
    }
}