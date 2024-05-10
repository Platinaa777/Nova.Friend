using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.Errors;

namespace Nova.Friend.Domain.FriendRequestAggregate.ValueObjects;

public class RequestId : ValueObject, IEquatable<RequestId>
{
    public string Value { get; }

    private RequestId(Guid invitationId)
    {
        Value = invitationId.ToString();
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public bool Equals(RequestId? other) => base.Equals(other);

    public static Result<RequestId> Create(string friendRequestId)
    {
        if (!Guid.TryParse(friendRequestId, out var id))
        {
            return Result.Failure<RequestId>(FriendRequestError.InvalidFriendRequestId);
        }

        return new RequestId(id);
    }
}