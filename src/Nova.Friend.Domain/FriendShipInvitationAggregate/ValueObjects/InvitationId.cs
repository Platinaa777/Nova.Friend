using DomainDrivenDesign.Abstractions;
using Nova.Friend.Domain.Errors;

namespace Nova.Friend.Domain.FriendShipInvitationAggregate.ValueObjects;

public class InvitationId : ValueObject, IEquatable<InvitationId>
{
    public string Value { get; }

    private InvitationId(Guid invitationId)
    {
        Value = invitationId.ToString();
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public bool Equals(InvitationId? other) => other is not null && other.Equals(this);

    public static Result<InvitationId> Create(string invitationId)
    {
        if (!Guid.TryParse(invitationId, out var id))
        {
            return Result.Failure<InvitationId>(InvitationError.InvalidInvitationId);
        }

        return new InvitationId(id);
    }
}