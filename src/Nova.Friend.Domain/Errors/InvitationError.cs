using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Domain.Errors;

public class InvitationError : Error
{
    public static Error InvalidInvitationId = new("Invitation.Identity", "Invalid invitation id");
    public static Error NotExistingStatusType = new("Invitation.Status", "This type of status does not exists");
    public static Error EmptyStatusType = new("Invitation.Status", "Invitation status must be not empty");
    
    public InvitationError(string code, string message) : base(code, message)
    {
    }
}