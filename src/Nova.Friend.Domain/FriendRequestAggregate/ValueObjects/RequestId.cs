using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.Errors;

namespace Nova.Friend.Domain.FriendRequestAggregate.ValueObjects;

public class RequestId : Id, IEquatable<RequestId>
{
    private RequestId(Guid invitationId) : base(invitationId)
    {
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public bool Equals(RequestId? other) => base.Equals(other);

    public new static Result<RequestId> Create(string friendRequestId)
    {
        if (!Guid.TryParse(friendRequestId, out var id))
        {
            return Result.Failure<RequestId>(FriendRequestError.InvalidFriendRequestId);
        }

        return new RequestId(id);
    }
}