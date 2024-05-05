using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Domain.FriendShipInvitationAggregate.Enumerations;

public class InvitationStatus : Enumeration, IEquatable<InvitationStatus>
{
    public static readonly InvitationStatus Pending = new InvitationStatus(1, "Pending");
    public static readonly InvitationStatus Accepted = new InvitationStatus(2, "Accepted");
    public static readonly InvitationStatus Rejected = new InvitationStatus(3, "Rejected");
    public static readonly InvitationStatus Canceled = new InvitationStatus(4, "Canceled");

    private InvitationStatus(int id, string name) : base(id, name)
    {
    }
    
    public static InvitationStatus? FromName(string? name)
    {
        var collection = GetAll<InvitationStatus>();
        foreach (var invitationStatus in collection)
        {
            if (invitationStatus.Name == name)
                return invitationStatus;
        }

        return null;
    }
    
    public static InvitationStatus? FromValue(int id)
    {
        var collection = GetAll<InvitationStatus>();
        foreach (var invitationStatus in collection)
        {
            if (invitationStatus.Id == id)
                return invitationStatus;
        }

        return null;
    }

    public bool Equals(InvitationStatus? other) => other is not null && other.Id == Id;

    public static bool operator ==(InvitationStatus left, InvitationStatus right) => left.Equals(right);

    public static bool operator !=(InvitationStatus left, InvitationStatus right) => !(left == right);
}