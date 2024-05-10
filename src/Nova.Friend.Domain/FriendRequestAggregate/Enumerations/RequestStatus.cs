using DomainDrivenDesign.Abstractions;

namespace Nova.Friend.Domain.FriendRequestAggregate.Enumerations;

public class RequestStatus : Enumeration, IEquatable<RequestStatus>
{
    public static readonly RequestStatus Pending = new RequestStatus(1, "Pending");
    public static readonly RequestStatus Accepted = new RequestStatus(2, "Accepted");
    public static readonly RequestStatus Rejected = new RequestStatus(3, "Rejected");
    public static readonly RequestStatus Canceled = new RequestStatus(4, "Canceled");

    private RequestStatus(int id, string name) : base(id, name)
    {
    }
    
    public static RequestStatus? FromName(string? name)
    {
        var collection = GetAll<RequestStatus>();
        foreach (var invitationStatus in collection)
        {
            if (invitationStatus.Name == name)
                return invitationStatus;
        }

        return null;
    }
    
    public static RequestStatus? FromValue(int id)
    {
        var collection = GetAll<RequestStatus>();
        foreach (var invitationStatus in collection)
        {
            if (invitationStatus.Id == id)
                return invitationStatus;
        }

        return null;
    }

    public bool Equals(RequestStatus? other) => other is not null && other.Id == Id;
    public override bool Equals(object obj) => obj is RequestStatus other && other.Equals(this);
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name);
    }

    public static bool operator ==(RequestStatus left, RequestStatus right) => left.Equals(right);

    public static bool operator !=(RequestStatus left, RequestStatus right) => !(left == right);
}